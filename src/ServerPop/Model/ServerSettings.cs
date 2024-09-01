namespace ServerPop.Model;

public class ServerSettings
{
    public string BotApiKey { get; set; }
    
    public int DataProcessingIntervalMs { get; set; }
    
    public int UserSettingsProcessingIntervalMs { get; set; }

    public ulong ServerStatusMessageId { get; set; }
}
