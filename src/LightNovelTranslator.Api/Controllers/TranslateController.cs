using LightNovelTranslator.Core;
using LightNovelTranslator.Core.Interfaces;
using LightNovelTranslator.Docx;
using Microsoft.AspNetCore.Mvc;

namespace LightNovelTranslator.Api.Controllers;

public class TranslateController : BaseController
{
    private string _input;
    private string _output;

    private IDocumentReader _reader;
    private IDocumentWriter _writer;
    private ITranslationProgressStore _progressStore;
    private ITranslator _translator;

    TranslateController(IDocumentReader reader, 
        IDocumentWriter writer, 
        ITranslationProgressStore progressStore, 
        ITranslator translator)
    {
        _translator = translator;
        _progressStore = progressStore;
        _writer = writer;
        _reader = reader;
    }

    [HttpPost("translate")]
    public async Task<IActionResult> Translate([FromBody] TranslationJobRequest request)
    {
        request.OutputPath = Helper.ResolveOutputPath(request.InputPath, request.OutputPath, request.Language);
        
        await ProcessTranslationJob(request.InputPath, request.OutputPath);
        
        return Ok("OK");
    }


    [HttpPost("translate-dir")]
    public async Task<IActionResult> TranslateDir([FromBody] TranslationJobRequest request)
    {
        var dir = Helper.GetDocumentsPaths(request.InputPath, request.Extession);
        foreach (var path in dir)
        {
            var outputPath = Helper.ResolveOutputPath(path, request.OutputPath, request.Language);
            await ProcessTranslationJob(path, outputPath);
            //Todo rise event for finished translation
        }
        return Ok("猫　WiP 猫");
    }

    public async Task ProcessTranslationJob(string inputPath, string outputPath)
    {
        var docTranslate = new DocxTranslator(
            _translator,
            _progressStore,
            inputPath,
            outputPath);
        
        var document = await _reader.ReadAsync(inputPath);
        var translatedDocument = await docTranslate.TranslateAsync(document);
        await _writer.WriteAsync(inputPath, outputPath, translatedDocument);
    }
    
    public sealed class TranslationJobRequest
    {
        public string InputPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public string Language { get; set; } = "Pl";
        public string Model { get; set; } = string.Empty;
        public string Extession { get; set; } = string.Empty;
    }
}