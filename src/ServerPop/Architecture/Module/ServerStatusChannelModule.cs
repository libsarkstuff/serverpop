using arkdedicated;
using arkdedicated.Extension;
using Discord;
using Discord.WebSocket;
using serverpop.Enum;
using ServerPop.Interface;
using ServerPop.Model;
using ServerPop.Model.WatchedServers;

namespace ServerPop.Architecture.Module;

public class ServerStatusChannelModule : IChannelModule
{
    public SocketGuild DiscordServer { get; set; }
    
    public string ChannelName { get; set; }
    
    public UserSettings UserSettings { get; set; }
    
    private string ServerStatusTemplate { get; set; }

    private ulong ServerStatusMessageId { get; set; }

    public ServerStatusChannelModule(SocketGuild discordServer, string channelName, UserSettings userSettings, ulong serverStatusMessageId)
    {
        DiscordServer = discordServer;

        ChannelName = channelName;

        UserSettings = userSettings;
        
        ServerStatusTemplate = "## Server Status\n\n### Tribe\n{tribeServers}\n\n### Enemy\n{enemyServers}\n\n### Watched\n{watchedServers}\n\n### Highest Populations\n{highPopServers}";

        ServerStatusMessageId = serverStatusMessageId;
    }

    internal async Task<IEnumerable<IWatchedServer>> FilterWatchedServers(IEnumerable<ServerInfo> serverInfos)
    {
        var watchedServers = new List<IWatchedServer>();
        foreach (var tribeServer in UserSettings.TribeServers.OrderByDescending(x => x.Name))
        {
            var foundServer = serverInfos.FirstOrDefault(x => x.Name.ToLower().Contains(tribeServer.Name.ToLower()));

            tribeServer.NumPlayers = foundServer == null ? -1 : foundServer.NumPlayers;

            watchedServers.Add(tribeServer);
        }
        
        foreach (var enemyServer in UserSettings.EnemyServers.OrderByDescending(x => x.Name))
        {
            var foundServer = serverInfos.FirstOrDefault(x => x.Name.ToLower().Contains(enemyServer.Name.ToLower()));

            enemyServer.NumPlayers = foundServer == null ? -1 : foundServer.NumPlayers;

            watchedServers.Add(enemyServer);
        }
        
        foreach (var watchServer in UserSettings.WatchServers.OrderByDescending(x => x.Name))
        {
            var foundServer = serverInfos.FirstOrDefault(x => x.Name.ToLower().Contains(watchServer.Name.ToLower()));

            watchServer.NumPlayers = foundServer == null ? -1 : foundServer.NumPlayers;

            watchedServers.Add(watchServer);
        }

        return watchedServers;
    }
    
    internal async Task<IEnumerable<ServerInfo>> FilterHighPopServers(IEnumerable<ServerInfo> serverInfos)
    {
        return serverInfos.ArkServers().OrderByDescending(x => x.NumPlayers).Take(7);
    }

    internal async Task<string> CompileServerStatusUpdate(IEnumerable<IWatchedServer> watchedServers, IEnumerable<ServerInfo> highPopServers)
    {
        var tribeServers = watchedServers.Where(x => x is TribeServer);
        var enemyServers = watchedServers.Where(x => x is EnemyServer);
        var watchServers = watchedServers.Where(x => x is WatchedServer);
        
        var tribeServerOutput = tribeServers.Cast<TribeServer>()
            .Select(x => 
                x.NumPlayers == -1 ? $":scream: {x.Name} (Server Offline) {GetAlertText(x)}"
                    : x.NumPlayers > 67 ? $":red_circle: {x.Name} ({x.NumPlayers} Online) {GetAlertText(x)}"
                    : x.NumPlayers > 60 ? $":orange_circle: {x.Name} ({x.NumPlayers} Online) {GetAlertText(x)}"
                    : x.NumPlayers > 40 ? $":yellow_circle: {x.Name} ({x.NumPlayers} Online) {GetAlertText(x)}"
                    : $":green_circle: {x.Name} ({x.NumPlayers} Online) {GetAlertText(x)}");
        
        var enemyServerOutput = enemyServers.Cast<EnemyServer>()
            .Select(x => 
                x.NumPlayers == -1 ? $":scream: {x.Name} (Server Offline) - {x.Desc} {GetAlertText(x)}"
                    : x.NumPlayers > 67 ? $":red_circle: {x.Name} ({x.NumPlayers} Online) - {x.Desc} {GetAlertText(x)}"
                    : x.NumPlayers > 60 ? $":orange_circle: {x.Name} ({x.NumPlayers} Online) - {x.Desc} {GetAlertText(x)}"
                    : x.NumPlayers > 40 ? $":yellow_circle: {x.Name} ({x.NumPlayers} Online) - {x.Desc} {GetAlertText(x)}"
                    : $":green_circle: {x.Name} ({x.NumPlayers} Online) - {x.Desc} {GetAlertText(x)}");
        
        var watchServerOutput = watchServers.Cast<WatchedServer>()
            .Select(x => 
                x.NumPlayers == -1 ? $":scream: {x.Name} (Server Offline) - {x.Desc} {GetAlertText(x)}"
                    : x.NumPlayers > 67 ? $":red_circle: {x.Name} ({x.NumPlayers} Online) - {x.Desc} {GetAlertText(x)}"
                    : x.NumPlayers > 60 ? $":orange_circle: {x.Name} ({x.NumPlayers} Online) - {x.Desc} {GetAlertText(x)}"
                    : x.NumPlayers > 40 ? $":yellow_circle: {x.Name} ({x.NumPlayers} Online) - {x.Desc} {GetAlertText(x)}"
                    : $":green_circle: {x.Name} ({x.NumPlayers} Online) - {x.Desc} {GetAlertText(x)}");

        var highPopServerOutput = highPopServers.Select(x => $"({x.NumPlayers} Online) {x.Name}");
        
        var compiledMarkdown = ServerStatusTemplate.Replace("{tribeServers}", string.Join(Environment.NewLine, tribeServerOutput))
            .Replace("{enemyServers}", string.Join(Environment.NewLine, enemyServerOutput))
            .Replace("{watchedServers}", string.Join(Environment.NewLine, watchServerOutput))
            .Replace("{highPopServers}", string.Join(Environment.NewLine, highPopServerOutput));
        
        return compiledMarkdown;
    }

    private string GetAlertText(IWatchedServer watchedServer)
    {
        var alertsForServer = this.UserSettings.AlertConditions.Where(x => x.ServerName.ToLower() == watchedServer.Name);

        if (!alertsForServer.Any())
        {
            return string.Empty;
        }

        var greaterAlerts = alertsForServer.Where(x => x.AlertOperator == AlertOperator.GreaterThan && watchedServer.NumPlayers > x.Volume);
        var lesserAlerts = alertsForServer.Where(x => x.AlertOperator == AlertOperator.LessThan && watchedServer.NumPlayers < x.Volume);
        var equalAlerts = alertsForServer.Where(x => x.AlertOperator == AlertOperator.EqualTo && watchedServer.NumPlayers == x.Volume);
        var alert = greaterAlerts.Union(lesserAlerts).Union(equalAlerts).FirstOrDefault();

        if (alert != null)
        {
            var @operator = alert.AlertOperator == AlertOperator.EqualTo ? "=" :
                alert.AlertOperator == AlertOperator.LessThan ? "<" :
                alert.AlertOperator == AlertOperator.GreaterThan ? ">" : "?";

            return $" @botping (pop {@operator} {alert.Volume})";
        }

        return string.Empty;
    }

    internal async Task SendServerStatusUpdate(string markdown)
    {
        var serverStatusChannel = DiscordServer.TextChannels.FirstOrDefault(x => x.Name.Contains("server-status"));

        if (serverStatusChannel == null)
        {
            return;
        }

        // prime new server
        //await serverStatusChannel.SendMessageAsync(".");
        //return;

        TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
        int secondsSinceEpoch = (int)t.TotalSeconds;
        
        var footer = $"{Environment.NewLine}{Environment.NewLine}*Updated <t:{secondsSinceEpoch}:R>*";
        var messageToSend = markdown + footer;

        var options = new RequestOptions()
        {
            RatelimitCallback = this.RateLimitCallback
        };
        
        try
        {
            var message = await serverStatusChannel.GetMessageAsync(ServerStatusMessageId);
            if (message == null)
            {
                message = await serverStatusChannel.SendMessageAsync(messageToSend);

                ServerStatusMessageId = message.Id;
            }

            await serverStatusChannel.ModifyMessageAsync(ServerStatusMessageId, (x) => x.Content = messageToSend, options);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception: {e.Message} {e.StackTrace}");
        }
    }

    private async Task RateLimitCallback(IRateLimitInfo info)
    {
        Console.WriteLine($"rate limit was at: [{info.Remaining}]");
    }
}
