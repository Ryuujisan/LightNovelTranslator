namespace LightNovelTranslator.Core.Models;

public sealed record TranslationQueueItem(

    string InputPath,
    string OutputPath,
    string Model,
    string RetryModel,
    string Language
);