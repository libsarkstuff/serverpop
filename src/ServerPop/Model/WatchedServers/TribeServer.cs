using ServerPop.Interface;

namespace ServerPop.Model.WatchedServers;

public class TribeServer : IWatchedServer
{
    public string Name { get; set; }
    public int NumPlayers { get; set; }

    public TribeServer(string name)
    {
        Name = name;
    }
}
