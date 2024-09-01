using arkdedicated;
using arkdedicated.Interface;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using serverpop.Enum;
using ServerPop.Architecture;
using ServerPop.Architecture.Module;
using ServerPop.Command;
using ServerPop.Enum;
using ServerPop.Model;

namespace ServerPop.Core;

public class DiscordBot
{
    private DiscordSocketClient Client { get; set; }
    
    private IServerInfoRepository ServerInfoRepo { get; set; }
    
    private EventBus EventBus { get; set; }
    
    private UserSettings UserSettings { get; set; }
    
    private ServerSettings ServerSettings { get; set; }
    
    private IServiceProvider ServiceProvider { get; set; }
    
    private ServerStatusChannelModule ServerStatusModule { get; set; }

    private CommandRouter CommandRouter { get; set; }
    
    public DiscordBot(DiscordSocketClient client, EventBus eventBus, IServerInfoRepository serverInfoRepo, ServerSettings serverSettings, UserSettings userSettings, IServiceProvider serviceProvider, CommandRouter commandRouter)
    {
        Client = client;

        EventBus = eventBus;

        ServerInfoRepo = serverInfoRepo;

        ServerSettings = serverSettings;

        UserSettings = userSettings;

        ServiceProvider = serviceProvider;

        CommandRouter = commandRouter;
    }

    public async Task Start()
    {
        //var config = new DiscordSocketConfig { MessageCacheSize = 100 };
        //_client = new DiscordSocketClient(config);
        
        Client.Ready += ClientReady;

        await Client.LoginAsync(TokenType.Bot, ServerSettings.BotApiKey);
        await Client.StartAsync();
        
        Client.Connected += () =>
        {
            Console.WriteLine("Bot is connected!");
            return Task.CompletedTask;
        };
        
        Console.WriteLine("Bot has been initialized and is awaiting input!");
		
        await Task.Delay(-1);
    }

    private async Task ClientReady()
    {
        Console.WriteLine("Bot is ready!");

        var guild = Client.Guilds.FirstOrDefault();

        if (guild == null)
        {
            throw new Exception("bot is not in a discord server");
        } 
        
        ServerStatusModule = new ServerStatusChannelModule(guild, "server-status", UserSettings, ServerSettings.ServerStatusMessageId);
        
        EventBus.GotServerInfos += async (sender, serverInfos) =>
        {
            if (Client.ConnectionState != ConnectionState.Connected)
            {
                return;
            }
            
            await ServerInfosUpdated(sender, serverInfos);
        };
        
        try
        {
            var addServerCommand = await CreateAddServerCommand();
            var result = await Client.Rest.CreateGuildCommand(addServerCommand.Build(), guild.Id);
            
            var removeServerCommand = await CreateRemoveServerCommand();
            await Client.Rest.CreateGuildCommand(removeServerCommand.Build(), guild.Id);

            var listAlertsCommand = await CreateListAlertsCommand();
            await Client.Rest.CreateGuildCommand(listAlertsCommand.Build(), guild.Id);

            var addAlertCommand = await CreateAddAlertCommand();
            await Client.Rest.CreateGuildCommand(addAlertCommand.Build(), guild.Id);

            var removeAlertCommand = await CreateRemoveAlertCommand();
            await Client.Rest.CreateGuildCommand(removeAlertCommand.Build(), guild.Id);

            Client.SlashCommandExecuted += this.CommandRouter.SlashCommandHandler;
        }
        catch(ApplicationCommandException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Message, Formatting.Indented);
            Console.WriteLine(json);
        }
    }
    
    private async Task<SlashCommandBuilder> CreateRemoveServerCommand()
    {
        var guildCommand = new SlashCommandBuilder();
        guildCommand.WithName("removeserver");
        guildCommand.WithDescription("Remove an ARK:Ascended server from watched lists");
        
        var serverName = new SlashCommandOptionBuilder()
            .WithName("servername")
            .WithDescription("Server to remove. Use short notation (e.g. d2344)")
            .WithRequired(true)
            .WithType(ApplicationCommandOptionType.String);
        guildCommand.AddOption(serverName);

        return guildCommand;
    }

    private async Task<SlashCommandBuilder> CreateAddServerCommand()
    {
        var guildCommand = new SlashCommandBuilder();
        guildCommand.WithName("addserver");
        guildCommand.WithDescription("Add an ARK:Ascended server to a watch group");
        
        var serverName = new SlashCommandOptionBuilder()
            .WithName("servername")
            .WithDescription("Server to add. Use short notation (e.g. d2344)")
            .WithRequired(true)
            .WithType(ApplicationCommandOptionType.String);
        guildCommand.AddOption(serverName);
            
        var watchedGroup = new SlashCommandOptionBuilder()
            .WithName("watchgroup")
            .WithDescription("Which sort of server is this? (e.g. tribe")
            .WithRequired(true)
            .AddChoice("Tribe", (int)WatchGroup.Tribe)
            .AddChoice("Enemy", (int)WatchGroup.Enemy)
            .AddChoice("Watched", (int)WatchGroup.Watched)
            .WithType(ApplicationCommandOptionType.Integer);
        guildCommand.AddOption(watchedGroup);
            
        var serverDesc = new SlashCommandOptionBuilder()
            .WithName("description")
            .WithDescription("can you describe the server? (only useful for Enemy/Watched servers)")
            .WithRequired(false)
            .WithType(ApplicationCommandOptionType.String);
        guildCommand.AddOption(serverDesc);

        return guildCommand;
    }

    private async Task<SlashCommandBuilder> CreateListAlertsCommand()
    {
        var guildCommand = new SlashCommandBuilder();
        guildCommand.WithName("listalerts");
        guildCommand.WithDescription("get a list of the population alerts");

        return guildCommand;
    }

    private async Task<SlashCommandBuilder> CreateAddAlertCommand()
    {
        var guildCommand = new SlashCommandBuilder();
        guildCommand.WithName("addalert");
        guildCommand.WithDescription("set up an alert for a server's population");

        var serverName = new SlashCommandOptionBuilder()
            .WithName("servername")
            .WithDescription("Server to add. Use short notation (e.g. d2344)")
            .WithRequired(true)
            .WithType(ApplicationCommandOptionType.String);
        guildCommand.AddOption(serverName);

        var @operator = new SlashCommandOptionBuilder()
            .WithName("operator")
            .WithDescription("do we care when the pop goes LessThan or GreaterThan the number?")
            .WithRequired(true)
            .AddChoice("less than", (int)AlertOperator.LessThan)
            .AddChoice("greater than", (int)AlertOperator.GreaterThan)
            .AddChoice("equal to", (int)AlertOperator.EqualTo)
            .WithType(ApplicationCommandOptionType.Integer);
        guildCommand.AddOption(@operator);

        var volume = new SlashCommandOptionBuilder()
            .WithName("volume")
            .WithDescription("the target population online number")
            .WithRequired(true)
            .WithMinValue(0)
            .WithMaxValue(70)
            .WithType(ApplicationCommandOptionType.Integer);
        guildCommand.AddOption(volume);

        return guildCommand;
    }

    private async Task<SlashCommandBuilder> CreateRemoveAlertCommand()
    {
        var guildCommand = new SlashCommandBuilder();
        guildCommand.WithName("removealert");
        guildCommand.WithDescription("removes an alert using the alert id from the /listalerts command");

        var id = new SlashCommandOptionBuilder()
            .WithName("listnumber")
            .WithDescription("alert number from the /listalerts command output")
            .WithRequired(true)
            .WithType(ApplicationCommandOptionType.Integer);
        guildCommand.AddOption(id);

        return guildCommand;
    }

    private async Task ServerInfosUpdated(object sender, IEnumerable<ServerInfo> serverInfos)
    {
        var filteredServers = await ServerStatusModule.FilterWatchedServers(serverInfos);

        var highPopServers = await ServerStatusModule.FilterHighPopServers(serverInfos);

        var updateMarkdown = await ServerStatusModule.CompileServerStatusUpdate(filteredServers, highPopServers);

        await ServerStatusModule.SendServerStatusUpdate(updateMarkdown);
    }

    private async Task ClearChannel(SocketTextChannel channel)
    {
        var messages = channel.GetMessagesAsync(100);

        await foreach (var message in messages)
        {
            foreach (var c in message)
            {
                try
                {
                    await channel.DeleteMessageAsync(c);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}