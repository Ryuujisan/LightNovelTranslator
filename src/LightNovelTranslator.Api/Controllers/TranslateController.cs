using LightNovelTranslator.Core;
using LightNovelTranslator.Core.Interfaces;
using LightNovelTranslator.Core.Models;
using LightNovelTranslator.Docx;
using Microsoft.AspNetCore.Mvc;

namespace LightNovelTranslator.Api.Controllers;

public class TranslateController(IServiceScopeFactory _scopeFactory, ITranslationQueue _queue) : BaseController
{
    /*
    private IDocumentReader _reader;
    private IDocumentWriter _writer;
    private ITranslationProgressStore _progressStore;
    private ITranslator _translator;
    private ITranslationProgressReporter _progressReporter;*/
/*
    public TranslateController(IDocumentReader reader, 
        IDocumentWriter writer, 
        ITranslationProgressStore progressStore, 
        ITranslationProgressReporter progressReporter,
        ITranslator translator)
    {
        _progressReporter = progressReporter;
        _translator = translator;
        _progressStore = progressStore;
        _writer = writer;
        _reader = reader;
    }
*/
    [HttpPost("job")]
    public async Task<IActionResult> Translate([FromBody] TranslationJobRequest request)
    {
        request.OutputPath = Helper.ResolveOutputPath(
            request.InputPath,
            request.OutputPath,
            request.Language);

        await _queue.EnqueueAsync(new TranslationQueueItem(
            request.InputPath,
            request.OutputPath,
            request.Model,
            request.RetryModel,
            request.Language));

        return Accepted(new { status = "猫 queued 猫" });
    }


    [HttpPost("job-dir")]
    public async Task<IActionResult> TranslateDir([FromBody] TranslationJobRequest request)
    {
        var dir = Helper.GetDocumentsPaths(request.InputPath, request.Extension);

        _ = Task.Run(async () =>
        {
            foreach (var path in dir)
            {
                var outputPath = Helper.ResolveOutputPath(path, request.OutputPath, request.Language);
                await ProcessTranslationJob(path, outputPath, request.Model, request.RetryModel);
                //Todo rise event for finished translation
            }
        });
        
        return Accepted(new
        {
            status = "猫 started 猫",
            files = dir.Length
        });
    }

    public async Task ProcessTranslationJob(string inputPath, string outputPath, string model, string retryModel)
    {
        using var scope = _scopeFactory.CreateScope();

        var reader = scope.ServiceProvider.GetRequiredService<IDocumentReader>();
        var writer = scope.ServiceProvider.GetRequiredService<IDocumentWriter>();
        var translator = scope.ServiceProvider.GetRequiredService<ITranslator>();
        var progressStore = scope.ServiceProvider.GetRequiredService<ITranslationProgressStore>();
        var progressReporter = scope.ServiceProvider.GetRequiredService<ITranslationProgressReporter>();
        
        var docTranslate = new DocxTranslator(
            translator,
            progressStore,
            progressReporter,
            inputPath,
            outputPath,
            model,
            retryModel);
        
        var document = await reader.ReadAsync(inputPath);

        try
        {
            var translatedDocument = await docTranslate.TranslateAsync(document);
            await writer.WriteAsync(inputPath, outputPath, translatedDocument);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await progressReporter.TranslationError(document.FileName, e.Message);
        }
    }
    
    public sealed class TranslationJobRequest
    {
        public string InputPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public string Language { get; set; } = "Pl";
        public string Model { get; set; } = "qwen3.5:9b";
        public string RetryModel { get; set; } = "huihui_ai/Qwen3.6-abliterated:27b";
        public string Extension { get; set; } = string.Empty;
    }
}