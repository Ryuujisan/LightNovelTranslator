namespace LightNovelTranslator.Core.Interfaces;

public interface ITranslationProgressReporter
{
    Task ProgressReport(int currentChunk, int totalChunks, string progressText);
    Task TranslationCompleted(string fileName);
    Task TranslationError(string fileName, string errorMessage);
}