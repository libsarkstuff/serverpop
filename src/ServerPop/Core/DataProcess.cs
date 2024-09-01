using System.Diagnostics;
using arkdedicated.Extension;
using arkdedicated.Interface;
using ServerPop.Architecture;
using ServerPop.Model;

namespace ServerPop.Core;

public class DataProcess
{
    private IServerInfoRepository ServerInfoRepo { get; set; }

    private EventBus EventBus { get; set; }
    
    private ServerSettings ServerSettings { get; set; }
    
    private UserSettings UserSettings { get; set; }

    public DataProcess(IServerInfoRepository serverInfoRepo, EventBus eventBus, ServerSettings serverSettings, UserSettings userSettings)
    {
        ServerInfoRepo = serverInfoRepo;
        
        EventBus = eventBus;

        ServerSettings = serverSettings;

        UserSettings = userSettings;
    }
    
    public async Task Start()
    {
        Console.WriteLine("Starting data collection...");

        await CollectData();

        Console.WriteLine("Data collection exiting!");
    }

    private async Task CollectData()
    {
        var periodicTask = PeriodicTaskFactory.Start(() =>
        {
            Update(); // on purpose
        }, intervalInMilliseconds: ServerSettings.DataProcessingIntervalMs);

        await periodicTask.ContinueWith(_ =>
        {
            Console.WriteLine("Data routine exited!");
        });
    }
 
    private async Task Update()
    {
        Console.WriteLine("Collecting data...");
        
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var serverInfos = (await ServerInfoRepo.GetServerInfos());
        stopwatch.Stop();
        Console.WriteLine($"Data collection complete in {stopwatch.ElapsedMilliseconds}ms");
        
        var goodServerInfos = serverInfos.ArkServers();

        EventBus.PublishServerInfos(goodServerInfos);
    }
}
