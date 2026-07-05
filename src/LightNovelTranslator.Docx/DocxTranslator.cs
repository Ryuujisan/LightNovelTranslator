using System.Diagnostics;
using LightNovelTranslator.Core.Interfaces;
using LightNovelTranslator.Core.Models;
using LightNovelTranslator.Ollama;

namespace LightNovelTranslator.Docx;

public class DocxTranslator : IDocumentTranslator
{
    public async Task<DocumentModel> TranslateAsync(DocumentModel document)
    {
        // For Test
        var doc = document.Paragraphs.Where(p => !p.HasImage)
            .Where(p => !string.IsNullOrWhiteSpace(p.Text))
            .Where(p => p.Index >= 20 && p.Index <= 40)
            .ToList();
        
        var translator = new OllamaTranslator();
        var sw = Stopwatch.StartNew();
        foreach (var d in doc)
        {
            d.Text = await translator.TranslateAsync(d.Text);
        }
        sw.Stop();
        Console.WriteLine($"Time: {sw.Elapsed}");
        Console.WriteLine($"Char : {doc.Sum(d => d.Text.Length)}");
        return document;
    }
}