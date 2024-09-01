using arkdedicated.Interface;
using Notworking;
using SteamServerApi.Models;
using SteamServerApi.Models.Packets;

namespace arkdedicated.Implementation;

public class ServerServices : IServerServices
{
    public async Task<List<ArkApiGameServer>> GetGameServerInfos(List<ArkApiGameServer> infoServers)
    {
        var gameServers = new List<ArkApiGameServer>();

        foreach (var infoServer in infoServers)
        {
            var gameServer = await this.GetServerInfo(infoServer);

            if (gameServer != null)
            {
                gameServer.InfoPort = infoServer.InfoPort;

                gameServers.Add(gameServer);
            }
        }

        return gameServers;
    }

    public async Task<ArkApiGameServer> GetServerInfo(ArkApiGameServer arkServer)
    {
        var bytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79, 0x00 };

        byte[] incBytes;

        try
        {
            incBytes = await SimpleUdp.SendAndRecieve(arkServer.Ip, arkServer.InfoPort, bytes);
        }
        catch (Exception e)
        {
            return null;
        }

        arkServer.Info = new ServerInfoResponsePacket(incBytes);

        arkServer.GamePort = arkServer.Info.Port;

        return arkServer;
    }
}