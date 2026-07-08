using LightNovelTranslator.Core.Models;

namespace LightNovelTranslator.Core.Interfaces;

public interface ITranslator
{
    Task<string> TranslateAsync(string text, string model);
    Task<string> RetryTranslateAsync(TranslationChunk chunk, string model);
}