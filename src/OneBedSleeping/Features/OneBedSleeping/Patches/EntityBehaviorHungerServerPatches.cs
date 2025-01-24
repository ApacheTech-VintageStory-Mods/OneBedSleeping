// ReSharper disable InconsistentNaming
// ReSharper disable RedundantAssignment

namespace ApacheTech.VintageMods.OneBedSleeping.Features.OneBedSleeping.Patches;

[HarmonyServerSidePatch]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class EntityBehaviorHungerServerPatches : WorldSettingsConsumer<OneBedSleepingSettings>
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EntityBehaviorHunger), "ReduceSaturation")]
    public static bool Patch_EntityBehaviorHunger_ReduceSaturation_Prefix(ICoreAPI ___api, ref float satLossMultiplier)
    {
        var sleepingMod = ___api.ModLoader.GetModSystem<ModSleeping>();
        if (sleepingMod.AllSleeping) satLossMultiplier *= Settings.SaturationMultiplier;
        return true;
    }
}