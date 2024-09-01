using serverpop.Model;
using ServerPop.Model.WatchedServers;

namespace ServerPop.Model;

public class UserSettings
{
    public static string GetPath()
    {
        var dir = (new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location)).Directory.ToString();

        return Path.Join(dir, "\\userSettings.json");
    }

    public List<TribeServer> TribeServers { get; set; }

    public List<WatchedServer> WatchServers { get; set; }

    public List<EnemyServer> EnemyServers { get; set; }

    public List<AlertCondition> AlertConditions { get; set; }

    public void Initialize()
    {
        TribeServers = new List<TribeServer>();
        WatchServers = new List<WatchedServer>();
        EnemyServers = new List<EnemyServer>();
        AlertConditions = new List<AlertCondition>();
    }

    public bool IsServerWatched(string serverName)
    {
        var exists = false;
        exists = TribeServers.Any(x => x.Name.ToLower() == serverName.ToLowerInvariant());
        exists = exists || EnemyServers.Any(x => x.Name.ToLower() == serverName.ToLowerInvariant());
        exists = exists || WatchServers.Any(x => x.Name.ToLower() == serverName.ToLowerInvariant());

        return exists;
    }
}
