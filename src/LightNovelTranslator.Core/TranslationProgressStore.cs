using System.Text.Json;
using System.Text.Json.Serialization;
using LightNovelTranslator.Core.Interfaces;
using LightNovelTranslator.Core.Models;

namespace LightNovelTranslator.Core;

public sealed class TranslationProgressStore : ITranslationProgressStore
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
        
    };

    public async Task<TranslationJobProgress> CreateAsync(
        string inputPath,
        string outputPath,
        string model,
        string retryModel,
        string language,
        IReadOnlyList<TranslationChunk> chunks)
    {
        var jobProgress = new TranslationJobProgress
        {
            InputPath = inputPath,
            OutputPath = outputPath,
            Model = model,
            RetryModel = retryModel,
            Language = language,
            ProgressPath = GetProgressPath(inputPath)
        };

        for (var i = 0; i < chunks.Count; i++)
        {
            jobProgress.Chunks.Add(new TranslationChunkResult
            {
                ChunkIndex = i,
                OriginalText = chunks[i].OriginalText,
                Status = EChunkStatus.Pending
            });
        }

        await SaveAsync(jobProgress);
        return jobProgress;
    }

    public async Task SaveAsync(TranslationJobProgress progress)
    {
        var json = JsonSerializer.Serialize(progress, _jsonOptions);

        var directory = Path.GetDirectoryName(progress.ProgressPath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(progress.ProgressPath, json);
    }

    public async Task<TranslationJobProgress> LoadAsync(string progressPath)
    {
        var json = await File.ReadAllTextAsync(progressPath);

        return JsonSerializer.Deserialize<TranslationJobProgress>(json, _jsonOptions)
               ?? throw new InvalidOperationException("Nie udało się wczytać progress JSON.");
    }

    public static string GetProgressPath(string inputPath)
    {
        return Path.ChangeExtension(inputPath, ".progress.json");
    }
}