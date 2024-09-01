using System.Data;
using System.Net.Http.Json;
using arkdedicated.Interface;
using System.Text.Json;

namespace arkdedicated.Implementation;

public class ServerInfoRepository : IServerInfoRepository
{
    public async Task<IEnumerable<ServerInfo>> GetServerInfos()
    {
        using HttpClient client = new();

        var serverInfos = await client.GetFromJsonAsync<List<ServerInfo>>("https://cdn2.arkdedicated.com/servers/asa/officialserverlist.json");

        if (serverInfos == null)
        {
            throw new NoNullAllowedException("Response from arkdedicated was null!");
        }

        return serverInfos;
    }
}