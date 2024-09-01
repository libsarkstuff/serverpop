using arkdedicated.Implementation;
using arkdedicated.Interface;
using Discord.WebSocket;
using ServerPop.Command.ServerStatus;
using ServerPop.Model;

namespace ServerPop.Command;

public class CommandRouter
{
    private readonly DiscordSocketClient _client;
    
    private UserSettings UserSettings { get; set; }
    
    private IServerInfoRepository ServerInfoRepo { get; set; }

    public CommandRouter(DiscordSocketClient client, IServerInfoRepository serverInfoRepo, UserSettings settings)
    {
        _client = client;

        ServerInfoRepo = serverInfoRepo;

        UserSettings = settings;
    }

    public async Task SlashCommandHandler(SocketSlashCommand command)
    {
        try
        {
            var suppressDefaultResponse = false;

            await command.RespondAsync("k sec..");

            var thisGuild = _client.Guilds.First(x => command.GuildId == x.Id);
            var writeRole = thisGuild.Roles.FirstOrDefault(x => x.Name.ToLower() == "write");
            if (!writeRole.Members.Any(x => x.Id == command.User.Id))
            {
                await command.ModifyOriginalResponseAsync((msg) => msg.Content= "only users with the Write role have access to bot config");

                return;
            }
            
            switch (command.Data.Name)
            {
                case "addserver":
                case "removeserver":
                case "addalert":
                case "removealert":
                case "listalerts":
                    suppressDefaultResponse = await ServerStatusRouter.Execute(command, UserSettings, ServerInfoRepo);
                    break;
                default:
                    await command.ModifyOriginalResponseAsync((msg) => msg.Content= "not sure what you want..");
                    return;
            }

            if (!suppressDefaultResponse)
            {
                await command.ModifyOriginalResponseAsync((msg) => msg.Content = "job done, boss.");
            }
        }
        catch (Exception e)
        {
            await command.ModifyOriginalResponseAsync((msg) => msg.Content= "something went wrong and it didn't work :(");
            Console.WriteLine(e);
            throw;
        }
    }
}
