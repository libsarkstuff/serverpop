using Discord.WebSocket;

namespace ServerPop.Interface;

public interface IChannelModule
{
    SocketGuild DiscordServer { get; set; } 
    
    string ChannelName { get; set; }
}