using Discord.WebSocket;
using ServerPop.Model;
using System.Text;
using serverpop.Enum;

namespace serverpop.Command.ServerStatus
{
    internal class ServerStatusListAlertCommand
    {
        public static async Task<bool> Execute(SocketSlashCommand command, UserSettings settings)
        {
            if (settings.AlertConditions.Count() < 1)
            {
                await command.ModifyOriginalResponseAsync((msg) => msg.Content = $"there are no alerts yet. you need to create some with /addalert");

                return true;
            }

            var alertOutput = new StringBuilder("the alerts configured rn are:" + Environment.NewLine + Environment.NewLine);

            for (var i = 0; i < settings.AlertConditions.Count(); i ++)
            {
                var condition = settings.AlertConditions[i];

                var alertOperator = string.Empty;
                switch (condition.AlertOperator)
                {
                    case AlertOperator.LessThan:
                        alertOperator = "<"; 
                        break;
                    case AlertOperator.GreaterThan:
                        alertOperator = ">"; 
                        break;
                    case AlertOperator.EqualTo: 
                        alertOperator = "="; 
                        break;
                }

                alertOutput.Append($"{i+1}) alert when {condition.ServerName} {alertOperator} {condition.Volume}{Environment.NewLine}");
            }

            await command.ModifyOriginalResponseAsync((msg) => msg.Content = alertOutput.ToString());

            return true;
        }
    }
}
