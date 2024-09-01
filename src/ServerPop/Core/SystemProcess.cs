using System.Text.Json;
using ServerPop.Architecture;
using ServerPop.Model;

namespace ServerPop.Core;

public class SystemProcess
{
    private UserSettings UserSettings { get; set; }
    
    private ServerSettings ServerSettings { get; set; }

    public SystemProcess(UserSettings settings, ServerSettings serverSettings)
    {
        UserSettings = settings;

        ServerSettings = serverSettings;
    }
    
    public async Task Start()
    {
        var periodicTask = PeriodicTaskFactory.Start(() =>
        {
            Update(); // on purpose
        }, intervalInMilliseconds: ServerSettings.UserSettingsProcessingIntervalMs);

        await periodicTask.ContinueWith(_ =>
        {
            Console.WriteLine("Data routine exited!");
        });
    }

    public async Task Update()
    {
        var userSettingsPath = UserSettings.GetPath();

        var settings = System.Text.Json.JsonSerializer.Serialize(UserSettings);

        await File.WriteAllTextAsync(userSettingsPath, settings);
    }
}
