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
        File.Copy(
            inputPath,
            outputPath,
            true);

        using var wordDoc =
            WordprocessingDocument.Open(
                outputPath,
                true);

        var body =
            wordDoc.MainDocumentPart!
                .Document
                .Body!;

        var paragraphs =
            body.Elements<Paragraph>()
                .ToList();

        foreach (var paragraphModel in document.Paragraphs)
        {
            if (paragraphModel.HasImage)
                continue;

            if (string.IsNullOrWhiteSpace(
                    paragraphModel.Text))
                continue;

            if (paragraphModel.Index >= paragraphs.Count)
                continue;

            var paragraph =
                paragraphs[paragraphModel.Index];

            ReplaceParagraphText(
                paragraph,
                paragraphModel.Text);
        }

        wordDoc.MainDocumentPart
            .Document
            .Save();
        
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