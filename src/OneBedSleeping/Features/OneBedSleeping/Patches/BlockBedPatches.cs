using OneBedSleeping.Features.OneBedSleeping.Systems;

namespace OneBedSleeping.Features.OneBedSleeping.Patches;

[HarmonyUniversalPatch]
public class BlockBedPatches
{
    [HarmonyPrefix]
    [HarmonyUniversalPatch(typeof(BlockBed), nameof(BlockBed.OnBlockInteractStart))]
    public static bool Patch_BlockBed_OnBlockInteractStart_Prefix(
        BlockBed __instance, IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref bool __result)
    {
        __result = true;
        var system = G.Uapi.ModLoader.GetModSystem<ObsNetworkSystem>();
        var allowed = system.SleepingAllowed;
        if (allowed) return true;

        if (world.Api is ICoreClientAPI capi)
        {
            capi.TriggerIngameError(__instance, "obs-sleeping-disallowed", G.T("OneBedSleeping", "SleepingDisallowed"));
        }
        else
        {
            byPlayer.Entity.TryUnmount();
        }
        return false;
    }
}