using System.Diagnostics;
using System.Text.RegularExpressions;
using LightNovelTranslator.Core.Interfaces;
using LightNovelTranslator.Core.Models;
using LightNovelTranslator.Ollama;

namespace LightNovelTranslator.Docx;

public class DocxTranslator : IDocumentTranslator
{
    private const string Separator = "<|LNT_PARAGRAPH_BREAK|>";

    private readonly ITranslator _translator;

    public DocxTranslator(ITranslator translator)
    {
        _translator = translator;
    }

    public async Task<DocumentModel> TranslateAsync(DocumentModel document)
    {
        var paragraphs = document.Paragraphs
            .Where(p => !p.HasImage)
            .Where(p => !string.IsNullOrWhiteSpace(p.Text))
            .Where(p => p.Text.Length > 2)
            .ToList();

        var chunks = DocumentChunker.CreateChunks(
            paragraphs,
            maxParagraphs: 12,
            maxChars: 2500);

        Console.WriteLine($"Chunks: {chunks.Count}");

        var sw = Stopwatch.StartNew();

        for (var i = 0; i < chunks.Count; i++)
        {
            var chunk = chunks[i];

            Console.WriteLine($"Translating chunk {i + 1}/{chunks.Count}");

            var translatedBlock = await _translator.TranslateAsync(chunk.OriginalText);

            translatedBlock = NormalizeResponse(translatedBlock, Separator);
            var translatedParagraphs =
                ParseNumberedParagraphs(translatedBlock);

            var hasParagraphMismatch =
                translatedParagraphs.Count != chunk.Paragraphs.Count;

            var hasEnglishLeak =
                HasEnglishLeak(translatedBlock);

            var isValid =
                !hasParagraphMismatch && !hasEnglishLeak;

            if (!isValid)
            {
                Console.WriteLine(
                    $"Retrying chunk {i + 1} with repair model...");

                translatedBlock =
                    await _translator.RetryTranslateAsync(chunk);

                translatedBlock =
                    NormalizeResponse(
                        translatedBlock,
                        Separator);

                translatedParagraphs =
                    ParseNumberedParagraphs(
                        translatedBlock);

                hasParagraphMismatch =
                    translatedParagraphs.Count != chunk.Paragraphs.Count;

                hasEnglishLeak =
                    HasEnglishLeak(translatedBlock);

                isValid =
                    !hasParagraphMismatch &&
                    !hasEnglishLeak;
            }

            for (var j = 0; j < chunk.Paragraphs.Count; j++)
            {
                chunk.Paragraphs[j].Text = translatedParagraphs[j];
            }
        }

        sw.Stop();

        Console.WriteLine($"Total time: {sw.Elapsed}");

        return document;
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
    
    private static void SaveFailedChunk(
        int chunkNumber,
        string originalText,
        string translatedText)
    {
        Directory.CreateDirectory("debug");

        File.WriteAllText(
            $"debug/failed_chunk_{chunkNumber}_input.txt",
            originalText);

        File.WriteAllText(
            $"debug/failed_chunk_{chunkNumber}_output.txt",
            translatedText);
    }
}