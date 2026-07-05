using System.Diagnostics;
using LightNovelTranslator.Core.Interfaces;
using LightNovelTranslator.Core.Models;
using LightNovelTranslator.Ollama;
using System.Text.RegularExpressions;

namespace LightNovelTranslator.Docx;

public class DocxTranslator : IDocumentTranslator
{
    private const string Separator = "<|LNT_PARAGRAPH_BREAK|>";

    private readonly ITranslator _translator;
    
    private static string NormalizeResponse(
        string response,
        string separator)
    {
        response = response.Trim();

        while (response.EndsWith(separator))
        {
            response = response[..^separator.Length]
                .Trim();
        }

        return response;
    }
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

            var promptText = chunk.OriginalText;

            var translatedBlock =
                await _translator.TranslateAsync(promptText);
           
            translatedBlock =
                NormalizeResponse(
                    translatedBlock,
                    Separator);

            var translatedParagraphs =
                ParseNumberedParagraphs(translatedBlock);

            if (translatedParagraphs.Count != chunk.Paragraphs.Count)
            {
                Console.WriteLine(
                    $"Warning: chunk {i + 1} paragraph count mismatch. " +
                    $"Expected {chunk.Paragraphs.Count}, got {translatedParagraphs.Count}");

                chunk.Paragraphs[0].Text = translatedBlock;

                for (var j = 1; j < chunk.Paragraphs.Count; j++)
                    chunk.Paragraphs[j].Text = string.Empty;
                
                continue;
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
}