namespace LightNovelTranslator.Core.Interfaces;

public interface ITranslator
{
    Task<string> TranslateAsync(string text);
}