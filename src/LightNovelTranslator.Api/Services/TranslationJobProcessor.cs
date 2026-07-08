using LightNovelTranslator.Core.Interfaces;
using LightNovelTranslator.Core.Models;
using LightNovelTranslator.Docx;

namespace LightNovelTranslator.Api.Services;

public sealed class TranslationJobProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TranslationJobProcessor(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task ProcessAsync(TranslationQueueItem job)
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
            job.InputPath,
            job.OutputPath,
            job.Model,
            job.RetryModel);

        var document = await reader.ReadAsync(job.InputPath);

        try
        {
            var translatedDocument = await docTranslate.TranslateAsync(document);
            await writer.WriteAsync(job.InputPath, job.OutputPath, translatedDocument);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await progressReporter.TranslationError(document.FileName, e.Message);
        }
    }
}