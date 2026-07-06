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
            //Model = "qwen3:32b",
            //Model ="huihui_ai/Qwen3.6-abliterated:27b",
            //Model = "jaahas/gemma-2-9b-it-abliterated",//"",//"",
            //Model = "maxwellb/gemma4-12b-it-dn",
            //Model = "richardyoung/qwen3-14b-abliterated",
            //Model = "huihui_ai/qwen2.5-coder-abliterate:14b",
           // Model = "wizardlm-uncensored",
            //Model = "dolphin-llama3",
            //Model = "mistral-nemo",
            //Model = "cas/aya-expanse-8b",
            //Model = "huihui_ai/qwen2.5-abliterate:14b",
            //Model = "gemma4:12b",
            //Model = "igorls/gemma-4-12B-it-heretic-GGUF",
           // Model = "huihui_ai/gemma-4-abliterated:12b",
           //Model = "gemma4:e4b",
           Model = "qwen3.5:9b",
           //Model = "huihui_ai/qwen3-abliterated:14b",
           //Model = "richardyoung/qwen2.5-14b-1m-heretic",
           // Model = "richardyoung/qwen2.5-14b-instruct-abliterated",
            //Model = "huihui_ai/qwen3.5-abliterated:9b",
            Prompt = prompt,
            Stream = false,
            Think = false,
            Options = new OllamaOptions
            {
                Temperature = 0.0,
                TopP = 1.0,
                RepeatPenalty = 1.0,
                NumCtx = 4096
            }
        };
        const int maxRetries = 5;

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    "http://localhost:11434/api/generate",
                    request);

                if (!response.IsSuccessStatusCode)
                {
                    var error =
                        await response.Content.ReadAsStringAsync();

                    throw new HttpRequestException(
                        $"Ollama error {(int)response.StatusCode}: {error}");
                }

                var result =
                    await response.Content
                        .ReadFromJsonAsync<OllamaGenerateResponse>();

                return result?.Response ?? string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Attempt {attempt}/{maxRetries} failed");

                Console.WriteLine(ex.Message);

                if (attempt == maxRetries)
                    throw;

                await Task.Delay(
                    TimeSpan.FromSeconds(attempt * 2));
            }
        }

        throw new InvalidOperationException();
    }
}