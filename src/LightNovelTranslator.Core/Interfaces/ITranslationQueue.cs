using LightNovelTranslator.Core.Models;

namespace LightNovelTranslator.Core.Interfaces;

public interface ITranslationQueue
{
    ValueTask EnqueueAsync(TranslationQueueItem item);
    ValueTask<TranslationQueueItem> DequeueAsync(CancellationToken cancellationToken);
}