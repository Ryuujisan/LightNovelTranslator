using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using LightNovelTranslator.Core.Interfaces;
using LightNovelTranslator.Core.Models;

namespace LightNovelTranslator.Docx;

public class DocxDocumentReader : IDocumentReader
{
    public async Task<DocumentModel> ReadAsync(string path)
    {
        var document = new DocumentModel()
        {
            FileName = Path.GetFileName(path)
        };
        
        using var wordDoc =
            WordprocessingDocument.Open(path, false);

        var body =
            wordDoc.MainDocumentPart!
                .Document
                .Body!;
        int index = 0;
        foreach (var paragraph in body.Elements<Paragraph>())
        {
            var text = paragraph.InnerText;

            var hasImage =
                paragraph.Descendants<DocumentFormat.OpenXml.Wordprocessing.Drawing>()
                    .Any();

            document.Paragraphs.Add(
                new DocumentParagraph
                {
                    Index = index++,
                    Text = text,
                    HasImage = hasImage
                });
        }

        return await Task.FromResult(document);
    }
}