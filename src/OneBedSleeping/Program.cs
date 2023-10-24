using Gantry.Services.HarmonyPatches.DependencyInjection;
using Gantry.Services.Network.DependencyInjection;

namespace ApacheTech.VintageMods.OneBedSleeping
{
    [UsedImplicitly]
    public sealed class Program : ModHost
    {
        public Program()
        {
#if DEBUG
            ModEx.DebugMode = true;
#endif
        }

        protected override void ConfigureServerModServices(IServiceCollection services, ICoreServerAPI sapi)
        {
            services.AddFileSystemService(o => o.RegisterSettingsFiles = true);
        }
        
        protected override void ConfigureUniversalModServices(IServiceCollection services, ICoreAPI api)
        {
            services.AddHarmonyPatchingService(o => o.AutoPatchModAssembly = true);
            services.AddNetworkService();
        }
    }
}