using System.Text.Json;
using arkdedicated.Implementation;
using arkdedicated.Interface;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using serverpop.Model;
using ServerPop.Architecture;
using ServerPop.Command;
using ServerPop.Core;
using ServerPop.Model;
using ServerPop.Model.WatchedServers;

namespace ServerPop;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var services = CreateServices();

        var dataProcess = services.GetRequiredService<DataProcess>();
        var discordProcess = services.GetRequiredService<DiscordBot>();
        var systemProcess = services.GetRequiredService<SystemProcess>();

        dataProcess.Start();
        discordProcess.Start();
        systemProcess.Start();

        Console.ReadLine();
    }

    private static ServiceProvider CreateServices()
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<DiscordSocketConfig>(sp =>
            {
                return new DiscordSocketConfig()
                {
                    GatewayIntents = Discord.GatewayIntents.AllUnprivileged,
                    AlwaysDownloadUsers = true
                };
            })
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<ServerSettings>(sp =>
            {
                var json = File.ReadAllText("./serverSettings.json");
                var settings = JsonSerializer.Deserialize<ServerSettings>(json);

                if (settings == null)
                {
                    throw new InvalidDataException("Missing server settings file serverSettings.json");
                }
                
                return settings;
            })
            .AddSingleton<UserSettings>(sp =>
            {
                var userSettingsPath = UserSettings.GetPath();

                if (!File.Exists(userSettingsPath))
                {
                    var newSettings = new UserSettings();
                    newSettings.Initialize();
                    
                    File.WriteAllText(userSettingsPath, JsonSerializer.Serialize(newSettings));
                }
                
                var json = File.ReadAllText(userSettingsPath);
                var settings = System.Text.Json.JsonSerializer.Deserialize<UserSettings>(json);

                if (settings == null)
                {
                    throw new InvalidDataException("Missing user settings file serverSettings.json");
                }

                if (settings.TribeServers == null || !settings.TribeServers.Any())
                {
                    settings.TribeServers = new List<TribeServer>();
                }

                if (settings.EnemyServers == null || !settings.EnemyServers.Any())
                {
                    settings.EnemyServers = new List<EnemyServer>();
                }

                if (settings.WatchServers == null || !settings.WatchServers.Any())
                {
                    settings.WatchServers = new List<WatchedServer>();
                }

                if (settings.AlertConditions == null || !settings.AlertConditions.Any())
                {
                    settings.AlertConditions = new List<AlertCondition>();
                }
                
                return settings;
            })
            .AddSingleton<IServerInfoRepository, ServerInfoRepository>()
            .AddSingleton<EventBus>()
            .AddSingleton<SystemProcess>()
            .AddSingleton<DataProcess>(sp =>
            {
                var serverInfoRepo = sp.GetRequiredService<IServerInfoRepository>();
                var eventBus = sp.GetRequiredService<EventBus>();
                var serverSettings = sp.GetRequiredService<ServerSettings>();
                var userSettings = sp.GetRequiredService<UserSettings>();
                
                return new DataProcess(serverInfoRepo, eventBus, serverSettings, userSettings);
            })
            .AddSingleton<CommandRouter>()
            .AddSingleton<DiscordBot>(sp =>
            {
                var eventBus = sp.GetRequiredService<EventBus>();
                var serverInfoRepo = sp.GetRequiredService<IServerInfoRepository>();
                var serverSettings = sp.GetRequiredService<ServerSettings>();
                var userSettings = sp.GetRequiredService<UserSettings>();
                var discordClient = sp.GetRequiredService<DiscordSocketClient>();
                var commandRouter = sp.GetRequiredService<CommandRouter>();

                return new DiscordBot(discordClient, eventBus, serverInfoRepo, serverSettings, userSettings, sp, commandRouter);
            })
            .BuildServiceProvider();

        return serviceProvider;
    }
}
