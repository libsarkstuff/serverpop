using arkdedicated;
using arkdedicated.Extension;
using arkdedicated.Interface;
using Discord.WebSocket;
using ServerPop.Enum;
using ServerPop.Interface;
using ServerPop.Model;

namespace ServerPop.Helper;

public class DiscordHelper
{
    public static async Task DeleteAllChannels(DiscordSocketClient client)
    {
        var guild = client.Guilds.FirstOrDefault();
        
        if (guild == null)
        {
            throw new Exception("No discord guild found!");
        }

        foreach (var channel in guild.TextChannels.Where(x => x.Category == null))
        {
            await channel.DeleteAsync();
        }
    }
    
    public static async Task ChannelSync(DiscordSocketClient client, IServerInfoRepository serverInfoRepo, UserSettings userSettings)
    {
        var guild = client.Guilds.FirstOrDefault();
        
        if (guild == null)
        {
            throw new Exception("No discord guild found!");
        }
        
        var allServerInfos = 
            (await serverInfoRepo.GetServerInfos())
            .ArkServers()
            .ToList();

        await SyncWatchedChannels(guild, allServerInfos, userSettings, WatchGroup.Tribe, userSettings.TribeServers.Cast<IWatchedServer>().ToList());
        await SyncWatchedChannels(guild, allServerInfos, userSettings, WatchGroup.Enemy, userSettings.EnemyServers.Cast<IWatchedServer>().ToList());
        await SyncWatchedChannels(guild, allServerInfos, userSettings, WatchGroup.Watched, userSettings.WatchServers.Cast<IWatchedServer>().ToList());
    }
    
    private static async Task SyncWatchedChannels(SocketGuild guild, List<ServerInfo> allServerInfos, UserSettings userSettings, WatchGroup watchGroup, List<IWatchedServer> userServers)
    {
        // ensure tribe category and channel
        var categoryName = System.Enum.GetName(typeof(WatchGroup), watchGroup);
        var category = guild.CategoryChannels.FirstOrDefault(x => x.Name == categoryName);
        if (category == null)
        {
            await guild.CreateCategoryChannelAsync(categoryName);
            
            category = guild.CategoryChannels.FirstOrDefault(x => x.Name == categoryName);
        }
        
        var servers = allServerInfos.Where(x => userServers.Any(y => x.Name.Contains(y.Name)));
        foreach (var server in servers)
        {
            var serverName = server.Name.ToLower();

            if (!guild.TextChannels.Any(x => x.Name == serverName))
            {
                await guild.CreateTextChannelAsync($"{server.Name.ArkShortName()}", x =>
                {
                    x.CategoryId = category.Id;
                });
            }
        }
    }
}