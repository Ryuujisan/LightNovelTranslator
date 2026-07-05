using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using LightNovelTranslator.Core.Interfaces;
using LightNovelTranslator.Core.Models;

namespace LightNovelTranslator.Docx;

public sealed class DocxDocumentWriter : IDocumentWriter
{
    public Task WriteAsync( string inputPath,
        string outputPath,
        DocumentModel document)
    {
        File.Copy(inputPath, outputPath, overwrite: true);

        using var wordDoc = WordprocessingDocument.Open(outputPath, true);

        var body = wordDoc.MainDocumentPart!
            .Document
            .Body!;

        var wordParagraphs = body
            .Elements<Paragraph>()
            .ToList();

        foreach (var paragraphModel in document.Paragraphs)
        {
            if (paragraphModel.HasImage)
                continue;

            if (string.IsNullOrWhiteSpace(paragraphModel.Text))
                continue;
            if (paragraphModel.Text.Contains("Table of Contents"))
                continue;

            if (paragraphModel.Text.Contains("Contents"))
                continue;

            if (paragraphModel.Index < 0 || paragraphModel.Index >= wordParagraphs.Count)
                continue;

            var wordParagraph = wordParagraphs[paragraphModel.Index];

            ReplaceParagraphText(
                wordParagraph,
                $"[TEXT TRANSLATED: {paragraphModel.Text}]");
        }

        wordDoc.MainDocumentPart.Document.Save();

        return Task.CompletedTask;
    }

    private static void ReplaceParagraphText(
        Paragraph paragraph,
        string newText)
    {
        paragraph.RemoveAllChildren<Run>();

        paragraph.AppendChild(
            new Run(
                new Text(newText)
                {
                    Space = SpaceProcessingModeValues.Preserve
                }));
    }
}