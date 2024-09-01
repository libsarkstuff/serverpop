using Discord.WebSocket;
using serverpop.Extensions;
using ServerPop.Enum;
using ServerPop.Model;
using ServerPop.Model.WatchedServers;

namespace ServerPop.Command.ServerStatus;

public static class ServerStatusAddCommand
{
    public static async Task<bool> Execute(SocketSlashCommand command, UserSettings settings)
    {
        var serverName = command.GetParam<string>("servername");
        var category = command.GetParam<string>("watchgroup");
        var description = command.GetParam<string>("description");

        if (string.IsNullOrWhiteSpace(serverName) || string.IsNullOrEmpty(category))
        {
            await command.ModifyOriginalResponseAsync((msg) => msg.Content = $"server name and category are required");

            return true;
        }
        
        var exists = settings.IsServerWatched(serverName);

        if (exists)
        {
            await command.ModifyOriginalResponseAsync((msg) => msg.Content = $"this server is already being watched");

            return true;
        }

        switch (System.Enum.Parse(typeof(WatchGroup), category))
        {
            case WatchGroup.Tribe:
                var ts = new TribeServer(serverName.ToLower());

                settings.TribeServers.Add(ts);

                break;

            case WatchGroup.Enemy:
                var es = new EnemyServer(serverName.ToLower());

                if (!string.IsNullOrWhiteSpace(description))
                {
                    es.Desc = description;
                }

                settings.EnemyServers.Add(es);

                break;

            case WatchGroup.Watched:
                var ws = new WatchedServer(serverName.ToLower());

                if (!string.IsNullOrWhiteSpace(description))
                {
                    ws.Desc = description;
                }

                settings.WatchServers.Add(ws);

                break;
        }

        return false;
    }
}
