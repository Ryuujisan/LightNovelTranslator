using System.Text.Json.Serialization;

namespace LightNovelTranslator.Ollama;

public sealed class OllamaOptions
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.1;

    [JsonPropertyName("top_p")]
    public double TopP { get; set; } = 0.9;

    [JsonPropertyName("repeat_penalty")]
    public double RepeatPenalty { get; set; } = 1.05;
    
    [JsonPropertyName("num_ctx")]
    public int NumCtx { get; set; } = 4096;
}