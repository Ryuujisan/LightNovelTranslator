using System.Diagnostics;
using System.Text.RegularExpressions;
using LightNovelTranslator.Core;
using LightNovelTranslator.Core.Interfaces;
using LightNovelTranslator.Core.Models;

namespace LightNovelTranslator.Docx;

public class DocxTranslator : IDocumentTranslator
{
    private const string Separator = "<|LNT_PARAGRAPH_BREAK|>";

    private readonly ITranslator _translator;
    private readonly ITranslationProgressStore _progressStore;
    private readonly ITranslationProgressReporter _progressReporter;

    private readonly string _inputPath;
    private readonly string _outputPath;
    private readonly string _model;
    private readonly string _retryModel;
    private readonly string _language;

    public DocxTranslator(
        ITranslator translator,
        ITranslationProgressStore progressStore,
        ITranslationProgressReporter progressReporter,
        string inputPath,
        string outputPath,
        string model,
        string retryModel,
        string language = "Polish")
    {
        _translator = translator;
        _progressStore = progressStore;
        _progressReporter = progressReporter;
        _inputPath = inputPath;
        _outputPath = outputPath;
        _model = model;
        _retryModel = retryModel;
        _language = language;
    }

    public async Task<DocumentModel> TranslateAsync(DocumentModel document)
    {
        var paragraphs = GetTranslatableParagraphs(document);

        var chunks = DocumentChunker.CreateChunks(
            paragraphs,
            maxParagraphs: 12,
            maxChars: 2500);

        Console.WriteLine($"Chunks: {chunks.Count}");

        var progress = await _progressStore.CreateAsync(
            _inputPath,
            _outputPath,
            _model,
            _retryModel,
            _language,
            chunks);

        var sw = Stopwatch.StartNew();

        await _progressReporter.ProgressReport(0, chunks.Count, document.FileName);

        for (var i = 0; i < chunks.Count; i++)
        {
            var chunk = chunks[i];

            var result = await TranslateChunkWithRetryAsync(chunk, i + 1);

            ApplyTranslatedParagraphs(chunk, result.Paragraphs);

            progress.Chunks[i].TranslatedText = result.TranslatedBlock;
            progress.Chunks[i].Status = result.IsValid
                ? EChunkStatus.Translated
                : EChunkStatus.Failed;

            await _progressReporter.ProgressReport(i + 1, chunks.Count, document.FileName);
            await _progressStore.SaveAsync(progress);
        }

        sw.Stop();

        Console.WriteLine($"Total time: {sw.Elapsed}");

        await _progressReporter.TranslationCompleted(document.FileName);

        return document;
    }

    public async Task<DocumentModel> ResumeAsync(
        DocumentModel document,
        TranslationJobProgress progress)
    {
        var paragraphs = GetTranslatableParagraphs(document);

        var chunks = DocumentChunker.CreateChunks(
            paragraphs,
            maxParagraphs: 12,
            maxChars: 2500);

        await _progressReporter.ProgressReport(0, chunks.Count, document.FileName);

        for (var i = 0; i < chunks.Count; i++)
        {
            var chunk = chunks[i];
            var progressChunk = progress.Chunks[i];

            if (progressChunk.Status == EChunkStatus.Translated)
            {
                var cachedParagraphs =
                    ParseNumberedParagraphs(progressChunk.TranslatedText);

                ApplyTranslatedParagraphs(chunk, cachedParagraphs);
                continue;
            }

            Console.WriteLine($"Resuming chunk {i + 1}/{chunks.Count}");

            progressChunk.Status = EChunkStatus.Translating;
            progressChunk.Attempts++;

            await _progressStore.SaveAsync(progress);

            var result = await TranslateChunkWithRetryAsync(chunk, i + 1);

            progressChunk.TranslatedText = result.TranslatedBlock;
            progressChunk.Status = result.IsValid
                ? EChunkStatus.Translated
                : EChunkStatus.Failed;

            await _progressReporter.ProgressReport(i + 1, chunks.Count, document.FileName);
            await _progressStore.SaveAsync(progress);

            if (!result.IsValid)
                continue;

            ApplyTranslatedParagraphs(chunk, result.Paragraphs);
        }

        await _progressReporter.TranslationCompleted(document.FileName);

        return document;
    }

    private async Task<ChunkTranslationResult> TranslateChunkWithRetryAsync(
        TranslationChunk chunk,
        int chunkNumber)
    {
        var translatedBlock = await _translator.TranslateAsync(
            chunk.OriginalText,
            _language,
            _model);

        var result = ValidateChunk(chunk, translatedBlock);

        if (result.IsValid)
            return result;

        Console.WriteLine($"Retrying chunk {chunkNumber} with repair model...");

        translatedBlock = await _translator.RetryTranslateAsync(
            chunk,
            _language,
            _retryModel);

        return ValidateChunk(chunk, translatedBlock);
    }

    private ChunkTranslationResult ValidateChunk(
        TranslationChunk chunk,
        string translatedBlock)
    {
        translatedBlock = NormalizeResponse(translatedBlock, Separator);

        var translatedParagraphs =
            ParseNumberedParagraphs(translatedBlock);

        var hasParagraphMismatch =
            translatedParagraphs.Count != chunk.Paragraphs.Count;

        var hasEnglishLeak = false;

        var needsEnglishLeakValidation =
            !_language.Equals("English", StringComparison.OrdinalIgnoreCase);

        if (needsEnglishLeakValidation)
        {
            hasEnglishLeak = HasEnglishLeak(translatedBlock);
        }

        var isValid =
            !hasParagraphMismatch &&
            !hasEnglishLeak;

        return new ChunkTranslationResult(
            translatedBlock,
            translatedParagraphs,
            isValid);
    }

    private static List<DocumentParagraph> GetTranslatableParagraphs(DocumentModel document)
    {
        return document.Paragraphs
            .Where(p => !p.HasImage)
            .Where(p => !string.IsNullOrWhiteSpace(p.Text))
            .Where(p => p.Text.Length > 2)
            .ToList();
    }

    private static void ApplyTranslatedParagraphs(
        TranslationChunk chunk,
        List<string> translatedParagraphs)
    {
        var count = Math.Min(
            chunk.Paragraphs.Count,
            translatedParagraphs.Count);

        for (var i = 0; i < count; i++)
        {
            chunk.Paragraphs[i].Text = translatedParagraphs[i];
        }
    }

    private static string NormalizeResponse(string response, string separator)
    {
        response = response.Trim();

        while (response.EndsWith(separator))
        {
            response = response[..^separator.Length].Trim();
        }

        return response;
    }

    private static List<string> ParseNumberedParagraphs(string text)
    {
        var matches = Regex.Matches(
            text,
            @"(?ms)^\[(\d+)\]\s*(.*?)(?=^\[\d+\]|\z)");

        return matches
            .OrderBy(m => int.Parse(m.Groups[1].Value))
            .Select(m => m.Groups[2].Value.Trim())
            .ToList();
    }

    private static bool HasEnglishLeak(string text)
    {
        var matches = Regex.Matches(
            text,
            @"\b(don't|doesn't|didn't|what|why|where|when|with|that|this|the|and|for|from|just|sorry|let's|sword|device|pervert|dormitory|bath|underwear|girl|guard|steam|roof|drag-ride|connection|huh|oops)\b",
            RegexOptions.IgnoreCase);

        return matches.Count >= 8;
    }

    private sealed record ChunkTranslationResult(
        string TranslatedBlock,
        List<string> Paragraphs,
        bool IsValid);
}