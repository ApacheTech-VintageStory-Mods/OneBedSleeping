// ReSharper disable InconsistentNaming
// ReSharper disable RedundantAssignment

namespace ApacheTech.VintageMods.OneBedSleeping.Features.OneBedSleeping.Patches
{
    [SettingsConsumer(EnumAppSide.Client)]
    [HarmonySidedPatch(EnumAppSide.Client)]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class ModSleepingClientPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ModSleeping), "WakeAllPlayers")]
        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public static bool Patch_ModSleeping_WakeAllPlayers_Prefix(ModSleeping __instance, ICoreClientAPI ___capi)
        {
            __instance.GameSpeedBoost = 0f;
            ___capi.World.Calendar.SetTimeSpeedModifier("sleeping", (int)__instance.GameSpeedBoost);
            var player = ___capi.World.Player;
            if (player.Entity?.MountedOn is BlockEntityBed) player.Entity.TryUnmount();
            __instance.AllSleeping = false;
            return false;
        }
    }
}