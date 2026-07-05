namespace LightNovelTranslator.Core.Models;

public sealed class DocumentModel
{
    public string FileName { get; set; } = string.Empty;

    public List<DocumentParagraph> Paragraphs { get; set; }
        = [];
}