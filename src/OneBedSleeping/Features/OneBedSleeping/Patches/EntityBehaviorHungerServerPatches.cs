using OneBedSleeping.Features.OneBedSleeping.Settings;

namespace OneBedSleeping.Features.OneBedSleeping.Patches;

public sealed class EntityBehaviorHungerServerPatches : GantrySettingsPatch<OneBedSleepingSettings>
{
    [HarmonyPrefix]
    [HarmonyServerPatch(typeof(EntityBehaviorHunger), "ReduceSaturation")]
    public static bool Patch_EntityBehaviorHunger_ReduceSaturation_Prefix(ref float satLossMultiplier)
    {
        var sleepingMod = G.Sapi.ModLoader.GetModSystem<ModSleeping>();
        if (sleepingMod.AllSleeping) satLossMultiplier *= Settings.SaturationMultiplier;
        return true;
    }
}