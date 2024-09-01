using Discord.WebSocket;
using ServerPop.Enum;
using ServerPop.Model.WatchedServers;
using ServerPop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using serverpop.Extensions;
using serverpop.Model;
using serverpop.Enum;

namespace serverpop.Command.ServerStatus
{
    internal class ServerStatusAddAlertCommand
    {
        public static async Task<bool> Execute(SocketSlashCommand command, UserSettings settings)
        {
            var serverName = command.GetParam<string>("servername");
            var @operator = command.GetParam<AlertOperator>("operator");
            var volume = command.GetParam<int>("volume");

            if (
                string.IsNullOrWhiteSpace(serverName) 
                || @operator == AlertOperator.None 
                || volume < 1 
                || volume > 70
            )
            {
                await command.ModifyOriginalResponseAsync((msg) => msg.Content = "some of the info you provided was weird");
                return true;
            }

            var exists = settings.IsServerWatched(serverName);

            if (!exists)
            {
                await command.ModifyOriginalResponseAsync((msg) => msg.Content = "you can only add an alert for a tribe, enemy or watched server");
                return true;
            }

            var alert = new AlertCondition(serverName, @operator, volume);

            settings.AlertConditions.Add(alert);

            return false;
        }
    }
}
