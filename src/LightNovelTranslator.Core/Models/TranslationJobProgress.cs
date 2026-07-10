namespace LightNovelTranslator.Core.Models;

public enum EChunkStatus
{
    Pending,
    Translating,
    Translated,
    Failed,
    Warning
}
public sealed class TranslationJobProgress
{
    public string InputPath { get; set; } = string.Empty;
    public string OutputPath { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string RetryModel { get; set; } = string.Empty;
    public string ProgressPath { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public List<TranslationChunkResult> Chunks { get; set; } = [];
}

public sealed class TranslationChunkResult
{
    public int ChunkIndex { get; set; }
    public string OriginalText { get; set; } = string.Empty;
    public string TranslatedText { get; set; } = string.Empty;
    public EChunkStatus Status { get; set; } = EChunkStatus.Pending;
    public int Attempts { get; set; }
    public string? ErrorMessage { get; set; }
}