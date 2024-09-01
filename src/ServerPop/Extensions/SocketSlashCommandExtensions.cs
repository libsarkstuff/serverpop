using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serverpop.Extensions
{
    public static class SocketSlashCommandExtensions
    {
        public static T GetParam<T>(this SocketSlashCommand command, string paramName)
        {
            var type = typeof(T);

            var option = command.Data.Options.FirstOrDefault(x => x.Name.ToLower() == paramName);

            if (option?.Value == null)
            {
                if (type == typeof(string))
                {
                    return (T)(object)String.Empty;
                }

                return default;
            }

            if (type.IsEnum)
            {
                try
                {
                    var enumVal = (T)System.Enum.Parse(typeof(T), option.Value.ToString());

                    return enumVal;
                }
                catch 
                {
                    return default;
                }
            }

            if (type == typeof(string))
            {
                return (T)(object)option.Value.ToString();
            }

            if (type == typeof(int))
            {
                int val;
                var success = Int32.TryParse(option.Value.ToString(), out val);

                return (T)(object)(success ? val : 0);
            }

            throw new NotImplementedException("GetParam used with unsupported type param");
        }
    }
}
