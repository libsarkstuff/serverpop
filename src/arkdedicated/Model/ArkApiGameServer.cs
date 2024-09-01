using SteamServerApi.Models.Packets;

namespace SteamServerApi.Models
{
	public class ArkApiGameServer
	{
		public string Ip { get; set; }

		public int GamePort { get; set; }

		public int InfoPort { get; set; }

		public ServerInfoResponsePacket Info { get; set; }

		public ArkApiGameServer(string ip, int infoPort)
		{
			this.Ip = ip;
			this.InfoPort = infoPort;
		}
	}
}
