namespace LightNovelTranslator.Core.Models;

public sealed class DocumentParagraph
{
    public int Index { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool HasImage { get; set; }
}