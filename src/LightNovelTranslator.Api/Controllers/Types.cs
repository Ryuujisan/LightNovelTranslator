using System.Text.Json.Serialization;

namespace LightNovelTranslator.Api.Controllers;

public enum EStatus
{
    Pending,
    Running,
    Completed,
    Failed
}

public class Job
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("inputPath")]
    public List<JobFile> InputPath { get; set; } = new();
}

public class JobFile
{
    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = string.Empty;

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public EStatus Status { get; set; } = EStatus.Pending;
}