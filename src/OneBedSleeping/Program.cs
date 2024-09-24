using Gantry.Core.Hosting;
using Gantry.Services.FileSystem.Hosting;
using Gantry.Services.HarmonyPatches.Hosting;
using Gantry.Services.Network.Hosting;

namespace ApacheTech.VintageMods.OneBedSleeping;

[UsedImplicitly]
public sealed class Program() : ModHost(debugMode:
#if DEBUG
    true
#else
    false
#endif
)
{

    protected override void ConfigureServerModServices(IServiceCollection services, ICoreServerAPI sapi)
    {
        services.AddFileSystemService(sapi, o => o.RegisterSettingsFiles = true);
    }
        
    protected override void ConfigureUniversalModServices(IServiceCollection services, ICoreAPI api)
    {
        services.AddHarmonyPatchingService(api, o => o.AutoPatchModAssembly = true);
        services.AddNetworkService(api);
    }
}