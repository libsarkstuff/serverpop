using arkdedicated.Interface;
using Discord.WebSocket;
using serverpop.Command.ServerStatus;
using ServerPop.Model;

namespace ServerPop.Command.ServerStatus;

public static class ServerStatusRouter
{
    public static async Task<bool> Execute(SocketSlashCommand command, UserSettings settings, IServerInfoRepository serverRepo)
    {
        switch (command.CommandName.ToLower())
        {
            case "addserver":
                return await ServerStatusAddCommand.Execute(command, settings);
            case "removeserver":
                return await ServerStatusRemoveCommand.Execute(command, settings);
            case "addalert":
                return await ServerStatusAddAlertCommand.Execute(command, settings);
            case "listalerts":
                return await ServerStatusListAlertCommand.Execute(command, settings);
            case "removealert":
                return await ServerStatusRemoveAlertCommand.Execute(command, settings);
            default:
                Console.WriteLine("Server Status command requested which does not exist");
                return false;
        }
    }
}
