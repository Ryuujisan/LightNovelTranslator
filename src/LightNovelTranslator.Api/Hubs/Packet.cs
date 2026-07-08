namespace LightNovelTranslator.Api.Hubs;

public enum EPacketType
{
    Progress,
    Complete,
    Error
}

public abstract class Packet;
public sealed class TranslationProgressPacket : Packet
{
    public int CurrentChunk {get; set;}
    public int TotalChunks {get; set;}
    public string? CurrentText {get; set;}
}

public sealed class TranslationCompletedPacket : Packet
{
    public string? FileName {get; set;}
}

public sealed class TranslationErrorPacket : Packet
{
    public string? FileName {get; set;}
    public string? ErrorMessage {get; set;}
}