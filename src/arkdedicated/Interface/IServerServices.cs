using SteamServerApi.Models;

namespace arkdedicated.Interface;

public interface IServerServices
{
    Task<List<ArkApiGameServer>> GetGameServerInfos(List<ArkApiGameServer> infoServers);

    Task<ArkApiGameServer> GetServerInfo(ArkApiGameServer arkServer);
}