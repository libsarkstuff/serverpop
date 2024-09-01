namespace arkdedicated.Interface;

public interface IServerInfoRepository
{
    Task<IEnumerable<ServerInfo>> GetServerInfos();
}
