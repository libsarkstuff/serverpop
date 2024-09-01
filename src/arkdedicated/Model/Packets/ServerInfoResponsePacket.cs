using Notworking;

namespace SteamServerApi.Models.Packets
{
	public class ServerInfoResponsePacket
	{
		public byte Protocol { get; set; }

		public string Name { get; set; }

		public string Map { get; set; }

		public string Folder { get; set; }

		public string Game { get; set; }

		public short Id { get; set; }

		public byte Players { get; set; }

		public byte MaxPlayers { get; set; }

		public byte Bots { get; set; }

		public byte ServerType { get; set; }

		public byte Environment { get; set; }

		public byte Visibility { get; set; }

		public byte VAC { get; set; }

		public string Version { get; set; }

		public byte ExtraDataFlag { get; set; }

		public short Port { get; set; }

		public long SteamId { get; set; }

		public long GameId { get; set; }

		// just for testing
		public ServerInfoResponsePacket()
		{
		}

		public ServerInfoResponsePacket(byte[] bytes)
		{
			var r = new PacketReader(bytes);

			this.Protocol = r.ReadByte();

			r.ReadByte(); // i
			r.ReadByte(); // have
			r.ReadByte(); // no
			r.ReadByte(); // idea
			r.ReadByte(); // wut these are

			this.Name = r.ReadString();

			this.Map = r.ReadString();

			this.Folder = r.ReadString();

			this.Game = r.ReadString();

			this.Id = r.ReadShort();

			this.Players = r.ReadByte();

			this.MaxPlayers = r.ReadByte();

			this.Bots = r.ReadByte();

			this.ServerType = r.ReadByte();

			this.Environment = r.ReadByte();

			this.Visibility = r.ReadByte();

			this.VAC = r.ReadByte();

			this.Version = r.ReadString();

			this.ExtraDataFlag = r.ReadByte();

			this.Port = r.ReadShort();

			this.SteamId = r.ReadLong();

			this.GameId = r.ReadLong();
		}
	}
}
