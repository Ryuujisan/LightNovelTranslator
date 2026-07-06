namespace LightNovelTranslator.Api.Services;

public sealed class OllamaService
{
    private readonly HttpClient _httpClient;

    public OllamaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/tags");

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string[]> ModelListAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<OllamaTagsResponse>("/api/tags");

            return response?.Models
                .Select(model => model.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToArray() ?? [];
        }
        catch
        {
            return [];
        }
    }
    
    private sealed class OllamaTagsResponse
    {
        public OllamaModel[] Models { get; set; } = [];
    }

    private sealed class OllamaModel
    {
        public string Name { get; set; } = "";
    }
}