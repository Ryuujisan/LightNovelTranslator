namespace LightNovelTranslator.Core.Models;

public enum ETranslationJobTypes
{
    Start , 
    Resume
}

public sealed record TranslationQueueItem(

    string InputPath,
    string OutputPath,
    string Model,
    string RetryModel,
    string Language,
    ETranslationJobTypes jobType
);
