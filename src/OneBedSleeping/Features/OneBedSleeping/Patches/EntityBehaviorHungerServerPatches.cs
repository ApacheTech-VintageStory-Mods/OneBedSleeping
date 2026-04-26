using OneBedSleeping.Features.OneBedSleeping.Settings;
using OneBedSleeping.Features.OneBedSleeping.Systems;

namespace OneBedSleeping.Features.OneBedSleeping.Patches;

public sealed class EntityBehaviorHungerServerPatches : GantrySettingsPatch<OneBedSleepingSettings>
{
    [HarmonyPrefix]
    [HarmonyServerPatch(typeof(EntityBehaviorHunger), "ReduceSaturation")]
    public static bool Patch_EntityBehaviorHunger_ReduceSaturation_Prefix(ref float satLossMultiplier)
    {
        var system = G.Sapi.ModLoader.GetModSystem<ObsControlSystem>();

        if (system.IsSleepActive)
        {
            satLossMultiplier *= Settings.SaturationMultiplier;
        }

        return true;
    }
}