using LightNovelTranslator.Core.Interfaces;

namespace LightNovelTranslator.Api.Services;

public sealed class TranslationWorker : BackgroundService
{
    private readonly ITranslationQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;

    public TranslationWorker(
        ITranslationQueue queue,
        IServiceScopeFactory scopeFactory)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var job = await _queue.DequeueAsync(stoppingToken);

            using var scope = _scopeFactory.CreateScope();

            var processor = scope.ServiceProvider
                .GetRequiredService<TranslationJobProcessor>();

            await processor.ProcessAsync(job);
        }
    }
}