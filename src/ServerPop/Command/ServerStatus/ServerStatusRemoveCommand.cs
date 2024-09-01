using Discord.WebSocket;
using serverpop.Extensions;
using ServerPop.Enum;
using ServerPop.Model;
using ServerPop.Model.WatchedServers;

namespace ServerPop.Command.ServerStatus;

public static class ServerStatusRemoveCommand
{
    public static async Task<bool> Execute(SocketSlashCommand command, UserSettings settings)
    {
        var serverName = command.GetParam<string>("servername");

        if (!settings.IsServerWatched(serverName))
        {
            await command.ModifyOriginalResponseAsync((msg) => msg.Content = $"we're not monitoring this server to begin with.. so.. done");

            return true;
        }

        if (settings.AlertConditions.Any(x => x.ServerName == serverName))
        {
            await command.ModifyOriginalResponseAsync((msg) => msg.Content = $"please remove any alerts on this server before you remove the actual server");

            return true;
        }

        settings.TribeServers = settings.TribeServers.Where(x => x.Name != serverName.ToLower()).ToList();
        settings.EnemyServers = settings.EnemyServers.Where(x => x.Name != serverName.ToLower()).ToList();
        settings.WatchServers = settings.WatchServers.Where(x => x.Name != serverName.ToLower()).ToList();

        return false;
    }
}
