using System.Collections.Generic;
using System.Linq;

namespace SteamServerApi.Models
{
	// This represents a physical computer rather than an 
	// instantiation of the Ark Server software.
	public class ArkApiInfoServer
	{
		public string Name { get; set; }

		public string Ip { get; set; }

		public readonly int[] ArkPorts = new[] { 27015, 27017, 27019, 27021 };

		public ArkApiInfoServer(string name, string ip)
		{
			this.Name = name;
			this.Ip = ip;
		}

		public IEnumerable<ArkApiGameServer> ExtractArks()
		{
			var arks = ArkPorts.Select(x => new ArkApiGameServer(this.Ip, x));

			return arks;
		}
	}
}
