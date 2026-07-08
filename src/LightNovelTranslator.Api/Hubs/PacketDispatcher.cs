namespace LightNovelTranslator.Api.Hubs;

public sealed class PacketDispatcher(IServiceProvider _serviceProvider)
{ //todo: PacketDispatcher
    public async Task DispatchAsync(string connectionId, ClientPacket packet)
    {
        switch (packet.Type)
        {
            default:
                throw new NotImplementedException();
        }
    }
}