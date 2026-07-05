using LightNovelTranslator.Core.Models;

namespace LightNovelTranslator.Core.Interfaces;

public interface IDocumentWriter
{
    Task WriteAsync(
        string inputPath,
        string outputPath,
        DocumentModel document);
}