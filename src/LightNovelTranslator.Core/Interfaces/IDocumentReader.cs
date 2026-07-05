using LightNovelTranslator.Core.Models;

namespace LightNovelTranslator.Core.Interfaces;

public interface IDocumentReader
{
    Task<DocumentModel> ReadAsync(string path);
}