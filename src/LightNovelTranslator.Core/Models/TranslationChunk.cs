namespace LightNovelTranslator.Core.Models;

public sealed class TranslationChunk
{
    public List<DocumentParagraph> Paragraphs { get; set; } = [];

    public string OriginalText
    {
        get
        {
            var parts = new List<string>();

            for (var i = 0; i < Paragraphs.Count; i++)
            {
                parts.Add($"[{i + 1}]\n{Paragraphs[i].Text}");
            }

            return string.Join("\n\n", parts);
        }
    }
}