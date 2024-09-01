namespace arkdedicated.Extension;

public static class ServerInfoListExtension
{
    public static IEnumerable<ServerInfo> ArkServers(this IEnumerable<ServerInfo> servers)
    {
        return servers.Where(x =>
            x.Name.ToLower().Contains("-pvp-")
            && !(
                x.Name.ToLower().Contains("console")
                || x.Name.ToLower().Contains("arkpocalypse")
                || x.Name.ToLower().Contains("smalltribes")
                || x.Name.ToLower().Contains("isolated")
                || x.Name.ToLower().Contains("sotf")
                || x.Name.ToLower().Contains("modded")
            ));
    }
}