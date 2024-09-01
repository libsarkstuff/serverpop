namespace arkdedicated;

public class ServerInfo
{
    public string SessionName { get; set;  }
    public string ModIDs { get; set; }
    public byte AllowDownloadItems { get; set; }
    public string SessionNameUpper { get; set; }
    public string SessionID { get; set; }
    public byte IsOfficial { get; set; }
    public int MaxPlayers { get; set; }
    public int Steelshield { get; set;  }
    public string ClusterId { get; set; }
    public string Sandbox { get; set; }
    public byte AllowDownloadChars { get; set; }
    public int NumPlayers { get; set; }
    public int Port { get; set; }
    public int DayTime { get; set; }
    public bool SOTFMatchStarted { get; set; }
    public string Name { get; set; }
    public string ModFileIDs { get; set; }
    public string IP { get; set; }
    public int ServerPing { get; set; }
    public string Service { get; set; }
    public int MinorBuildId { get; set; }
    public bool HasPassword { get; set; }
    public byte SessionIsPve { get; set; }
    public string MapName { get; set; }
    public byte Battleye { get; set; }
    public string LatencyPort { get; set; }
    public long LastUpdated { get; set; }
    public int BuildId { get; set; }
    public string SearchHandle { get; set; }
    public string PlatformType { get; set; }
    public string GameMode { get; set; }
}
