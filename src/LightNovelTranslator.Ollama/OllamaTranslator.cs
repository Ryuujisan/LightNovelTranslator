using System.Net.Http.Json;
using LightNovelTranslator.Core.Interfaces;

namespace LightNovelTranslator.Ollama;

public class OllamaTranslator : ITranslator
{
    private readonly HttpClient _httpClient;
    
    public OllamaTranslator()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(30)
        };
    }
    public async Task<string> TranslateAsync(string text)
    {
        var assembly = typeof(ITranslator).Assembly;

        using var stream =
            assembly.GetManifestResourceStream(
                "LightNovelTranslator.Core.Prompts.TranslationPrompt.txt");

        using var reader = new StreamReader(stream!);

        var promptTemplate = await reader.ReadToEndAsync();

        var prompt =
            promptTemplate.Replace("{{TEXT}}", text);

        var request = new OllamaGenerateRequest
        {
            Model = "qwen3:32b",
            Prompt = prompt,
            Stream = false,
            Think = false,
            Options = new OllamaOptions
            {
                Temperature = 0.0,
                TopP = 1.0,
                RepeatPenalty = 1.0,
            }
        };

        var response = await _httpClient.PostAsJsonAsync(
            "http://localhost:11434/api/generate",
            request);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>();

         return result?.Response ?? string.Empty;
    }
}