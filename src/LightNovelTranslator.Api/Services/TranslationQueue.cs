using System.Threading.Channels;
using LightNovelTranslator.Core.Interfaces;
using LightNovelTranslator.Core.Models;

namespace LightNovelTranslator.Api.Services;

public class TranslationQueue : ITranslationQueue
{
    private readonly Channel<TranslationQueueItem> _queue =
        Channel.CreateUnbounded<TranslationQueueItem>();

    public ValueTask EnqueueAsync(TranslationQueueItem item) =>
        _queue.Writer.WriteAsync(item);

    public ValueTask<TranslationQueueItem> DequeueAsync(CancellationToken cancellationToken) =>
        _queue.Reader.ReadAsync(cancellationToken);
}