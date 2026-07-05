using System.Text.Json.Serialization;

namespace LightNovelTranslator.Ollama;

public sealed class OllamaGenerateRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    [JsonPropertyName("stream")]
    public bool Stream { get; set; }

    [JsonPropertyName("think")]
    public bool Think { get; set; }

    [JsonPropertyName("options")]
    public OllamaOptions Options { get; set; } = new();
}