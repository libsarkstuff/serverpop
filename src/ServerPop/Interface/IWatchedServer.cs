namespace ServerPop.Interface;

public interface IWatchedServer
{
    public string Name { get; set; }
    
    public int NumPlayers { get; set; }
}
