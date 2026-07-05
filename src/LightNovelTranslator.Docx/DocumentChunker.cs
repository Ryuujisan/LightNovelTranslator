using LightNovelTranslator.Core.Models;

namespace LightNovelTranslator.Docx;

public static class DocumentChunker
{
    public static List<TranslationChunk> CreateChunks(
        List<DocumentParagraph> paragraphs,
        int maxParagraphs = 15,
        int maxChars = 3000)
    {
        var chunks = new List<TranslationChunk>();
        var current = new TranslationChunk();
        var currentChars = 0;

        foreach (var paragraph in paragraphs)
        {
            var textLength = paragraph.Text.Length;

            var wouldExceedParagraphs =
                current.Paragraphs.Count >= maxParagraphs;

            var wouldExceedChars =
                currentChars + textLength > maxChars;

            if (current.Paragraphs.Count > 0 &&
                (wouldExceedParagraphs || wouldExceedChars))
            {
                chunks.Add(current);
                current = new TranslationChunk();
                currentChars = 0;
            }

            current.Paragraphs.Add(paragraph);
            currentChars += textLength;
        }

        if (current.Paragraphs.Count > 0)
            chunks.Add(current);

        return chunks;
    }
}