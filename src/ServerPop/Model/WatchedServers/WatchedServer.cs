using ServerPop.Interface;

namespace ServerPop.Model.WatchedServers;

public class WatchedServer : IWatchedServer
{
    public string Name { get; set; }
    public int NumPlayers { get; set; }

    public string Desc { get; set; }

    public WatchedServer(string name, string desc = "")
    {
        Name = name;
        Desc = desc;
    }
}
