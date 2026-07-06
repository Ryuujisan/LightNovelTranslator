using LightNovelTranslator.Core.Models;

namespace LightNovelTranslator.Core.Interfaces;

public interface IDocumentTranslator
{
    Task<DocumentModel> TranslateAsync(
        DocumentModel document);

    Task<DocumentModel> ResumeAsync(DocumentModel document, TranslationJobProgress progress);
}