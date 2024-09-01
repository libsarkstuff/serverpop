using Discord.WebSocket;
using serverpop.Extensions;
using ServerPop.Model;

namespace serverpop.Command.ServerStatus
{
    internal class ServerStatusRemoveAlertCommand
    {
        public static async Task<bool> Execute(SocketSlashCommand command, UserSettings settings)
        {
            var listNumber = command.GetParam<int>("listnumber") - 1;

            if (settings.AlertConditions.Count() == 0 || listNumber < 0 || listNumber > settings.AlertConditions.Count())
            {
                await command.ModifyOriginalResponseAsync((msg) => msg.Content = $"i don't have an alert with that number in the list. use the /listalerts command to find a number");

                return true;
            }

            var theAlert = settings.AlertConditions[listNumber];

            settings.AlertConditions.Remove(theAlert);

            return false;
        }
    }
}
