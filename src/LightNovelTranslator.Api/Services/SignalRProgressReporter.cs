using LightNovelTranslator.Api.Hubs;
using LightNovelTranslator.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace LightNovelTranslator.Api.Services;

public sealed class SignalRProgressReporter : ITranslationProgressReporter
{
    private IHubContext<TranslateHub> _hub;
    private IDocumentTranslator _translator;

    public SignalRProgressReporter(IHubContext<TranslateHub> hub)
    {
        _hub = hub;
    }
    
    public async Task ProgressReport(int currentChunk, int totalChunks, string progressText) =>
        await _hub.Clients.All.SendAsync(
            EPacketType.Progress.ToString(), new TranslationProgressPacket()
            {
                CurrentChunk = currentChunk,
                TotalChunks = totalChunks,
                CurrentText = progressText
            }
        );

    public async Task TranslationCompleted(string fileName) =>
        await _hub.Clients.All.SendAsync(EPacketType.Complete.ToString(), new TranslationCompletedPacket(){ FileName = fileName});

    public async Task TranslationError(string fileName, string errorMessage) => await _hub.Clients.All.SendAsync(
        EPacketType.Error.ToString(), new TranslationErrorPacket() { FileName = fileName, ErrorMessage = errorMessage });
}