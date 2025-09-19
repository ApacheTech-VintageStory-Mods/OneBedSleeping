using OneBedSleeping.Settings;

namespace OneBedSleeping.Patches;

public sealed class EntityBehaviorHungerServerPatches : GantrySettingsPatch<OneBedSleepingSettings>
{
    [HarmonyPrefix]
    [HarmonyServerPatch(typeof(EntityBehaviorHunger), "ReduceSaturation")]
    public static bool Patch_EntityBehaviorHunger_ReduceSaturation_Prefix(ICoreAPI ___api, ref float satLossMultiplier)
    {
        var sleepingMod = ___api.ModLoader.GetModSystem<ModSleeping>();
        if (sleepingMod.AllSleeping) satLossMultiplier *= Settings.SaturationMultiplier;
        return true;
    }
}