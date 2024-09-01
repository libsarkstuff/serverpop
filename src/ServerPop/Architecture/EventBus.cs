using arkdedicated;

namespace ServerPop.Architecture;

public class EventBus
{
    public event EventHandler<ServerInfo> GotServerInfo = delegate { };
    
    public event EventHandler<IEnumerable<ServerInfo>> GotServerInfos = delegate { };

    public void PublishServerInfo(ServerInfo serverInfo)
    {
        GotServerInfo(this, serverInfo);
    }
    
    public void PublishServerInfos(IEnumerable<ServerInfo> serverInfos)
    {
        GotServerInfos(this, serverInfos);
    }
}