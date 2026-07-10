using LightNovelTranslator.Core.Models;

namespace LightNovelTranslator.Core.Interfaces;

public interface ITranslationProgressStore
{
    Task<TranslationJobProgress> CreateAsync(
        string inputPath,
        string outputPath,
        string model,
        string retryModel,
        string language,
        IReadOnlyList<TranslationChunk> chunks);

    Task SaveAsync(TranslationJobProgress progress);

    Task<TranslationJobProgress> LoadAsync(string progressPath);
}