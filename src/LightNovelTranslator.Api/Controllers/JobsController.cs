using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using LightNovelTranslator.Core;
using LightNovelTranslator.Core.Interfaces;
using LightNovelTranslator.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace LightNovelTranslator.Api.Controllers;

public class JobsController(ITranslationProgressStore _progressStore,
                            ITranslationQueue _queue) : BaseController
{
    const string TEMP_FOLDER = "LightNovelTranslator";
    const string INPUT_FOLDER = "input";
    const string OUTPUT_FOLDER = "output";
    const string JOBS_FOLDER = "jobs";
    const string JOB_FILE = "job.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    [HttpGet]
    public IActionResult Get()
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            TEMP_FOLDER,
            JOBS_FOLDER);
        
        if (!Directory.Exists(path))
        {
            return Ok(new List<JobDto>());
        }
        
        var jobsDir = Directory.GetDirectories(path);
        var jobs = new List<JobDto>();
        foreach (var jobDir in jobsDir)
        {
            var jobId = Path.GetFileName(jobDir);
            TryGetJobJson(jobId, out var job);
            jobs.Add(new JobDto
            {
                Id = jobId,
                Name = job?.Name ?? string.Empty
            });
        }
        
        return Ok(jobs);   
    }
    
    [HttpGet("{jobId}")]
    public async Task<IActionResult> Get(string jobId)
    {
        if(!TryGetJobJson(jobId, out var job))
        {
            return NotFound();
        }
        
        foreach (var file in job.InputPath)
        {
            try
            {
                var progress = await _progressStore.LoadAsync(TranslationProgressStore.GetProgressPath(file.Path));
                var maxChunks = progress.Chunks.Count;
                var translatedChunks = progress.Chunks.Count(c => c.Status == EChunkStatus.Translated);
                file.Status = maxChunks == translatedChunks ? EStatus.Completed : EStatus.Running;
                
                if (progress.Chunks.Any(c => c.Status == EChunkStatus.Failed))
                {
                    file.Status = EStatus.Failed;
                }
                else if (translatedChunks == maxChunks)
                {
                    file.Status = EStatus.Completed;
                }
                else if (translatedChunks > 0)
                {
                    file.Status = EStatus.Running;
                }
                else
                {
                    file.Status = EStatus.Pending;
                }
            }
            catch (Exception e)
            {
                file.Status = EStatus.Pending;
                continue;
            }
            
        }
        return Ok(job);
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start([FromBody] JobRequest request)
    {
        Console.WriteLine("start: " + request.Type);
        if (request.Files.Length == 0)
        {
            return BadRequest("No Configured Jobs");
        }

        switch (request.Type)
        {
            case ETranslationJobTypes.Start:
                await StartJob(request.Files, request.OutputPath, request.Model, request.RetryModel, request.Language);
                break;
            case ETranslationJobTypes.Resume:
                await ResumeJob(request.Files);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return Accepted(new { status = "猫 queued 猫" });
    }

    private async Task StartJob(JobFile[] file, string outputPath, string model, string retryModel, string langue )
    {

            foreach (var inputPath in file)
            {
                try
                {
                    var writePath = string.IsNullOrEmpty(outputPath)
                        ? Helper.ResolveOutputPath(inputPath.Path, inputPath.Path, langue)
                        : Helper.ResolveOutputPath(inputPath.Path, outputPath, langue);
                    await _queue.EnqueueAsync(new TranslationQueueItem(
                        inputPath.Path,
                        writePath,
                        model,
                        retryModel,
                        langue,
                        ETranslationJobTypes.Start));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    continue;
                }
            }
    }

    private async Task ResumeJob(JobFile[] files)
    {
        foreach (var file in files)
        {
            try
            {
                var progress = await _progressStore.LoadAsync(TranslationProgressStore.GetProgressPath(file.Path));
                await _queue.EnqueueAsync(new TranslationQueueItem(
                    file.Path,
                    progress.OutputPath,
                    progress.Model,
                    progress.RetryModel,
                    progress.Language,
                    ETranslationJobTypes.Resume));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
    private bool TryGetJobJson(string jobId, out Job job)
    {
        var jobPath = GetJobJsonPath(jobId);
        if (!System.IO.File.Exists(jobPath))
        {
            job = null;
            return false;
        }
        
        var json = System.IO.File.ReadAllText(jobPath);
        job = JsonSerializer.Deserialize<Job>(json, JsonOptions)
              ?? throw new InvalidOperationException($"Nie udało się wczytać pliku job JSON: {jobPath}");
        return true;
    }
    
    private string GetJobJsonPath(string jobId)
    {
        return Path.Combine(
            Path.GetTempPath(),
            TEMP_FOLDER,
            JOBS_FOLDER,
            jobId,
            JOB_FILE);
    }

    public class JobRequest
    {
        public JobFile[] Files { get; set; } = [];
        public string? Model { get; set; } = string.Empty;
        public string? RetryModel { get; set; } = string.Empty;
        public string? OutputPath { get; set; } = string.Empty;
        public string? Language { get; set; } = string.Empty;    
        public ETranslationJobTypes Type { get; set; }
    }
    
    public class JobDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}