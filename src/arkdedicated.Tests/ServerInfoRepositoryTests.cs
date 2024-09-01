using arkdedicated.Implementation;
using arkdedicated.Interface;

namespace arkdedicated.Tests;

public class Tests
{
    internal IServerInfoRepository ServerInfoRepo { get; set; }
    
    [SetUp]
    public void Setup()
    {
        this.ServerInfoRepo = new ServerInfoRepository();
    }

    [Test]
    public async Task Server_List_Returns_Servers()
    {
        var serverInfo = await this.ServerInfoRepo.GetServerInfos();
        
        Assert.Greater(serverInfo.Count(), 0);
        Assert.Pass();
    }
}