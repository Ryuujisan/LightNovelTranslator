using LightNovelTranslator.Core;
using LightNovelTranslator.Core.Interfaces;
using LightNovelTranslator.Docx;
using Microsoft.AspNetCore.Mvc;

namespace LightNovelTranslator.Api.Controllers;

public class TranslateController(IServiceScopeFactory _scopeFactory) : BaseController
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
        request.OutputPath = Helper.ResolveOutputPath(request.InputPath, request.OutputPath, request.Language);
        
        _ = Task.Run(() => ProcessTranslationJob(request.InputPath, request.OutputPath));

        return Accepted(new { status = "猫 started 猫" });
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
                await ProcessTranslationJob(path, outputPath);
                //Todo rise event for finished translation
            }
        });
        
        return Accepted(new
        {
            status = "猫 started 猫",
            files = dir.Length
        });
    }

    public async Task ProcessTranslationJob(string inputPath, string outputPath)
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
            outputPath);
        
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
        public string Model { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
    }
}