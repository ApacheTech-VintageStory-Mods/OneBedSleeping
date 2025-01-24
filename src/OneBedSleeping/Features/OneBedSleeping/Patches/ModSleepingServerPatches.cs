// ReSharper disable InconsistentNaming
// ReSharper disable RedundantAssignment

using Gantry.Services.BrighterChat.Commands;

namespace ApacheTech.VintageMods.OneBedSleeping.Features.OneBedSleeping.Patches;

[HarmonyServerSidePatch]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class ModSleepingServerPatches : WorldSettingsConsumer<OneBedSleepingSettings>
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ModSleeping), "AreAllPlayersSleeping")]
    public static bool Patch_ModSleeping_AreAllPlayersSleeping_Prefix(ICoreServerAPI ___sapi, ref bool __result)
    {
        var allPlayers = ___sapi.World.AllPlayersThatCouldSleep().ToList();
        var playersSleeping = allPlayers.SleepingPlayers().Count();
        var requiredNumberOfPlayers = GameMath.Clamp(Math.Ceiling(allPlayers.Count * Settings.PlayerThreshold), 1, allPlayers.Count);
        __result = playersSleeping >= requiredNumberOfPlayers;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ModSleeping), "ServerSlowTick")]
    public static bool Patch_ModSleeping_ServerSlowTick_Prefix(ModSleeping __instance, IServerNetworkChannel ___serverChannel)
    {
        var nowAllSleeping = __instance.AreAllPlayersSleeping();
        if (nowAllSleeping == __instance.AllSleeping) return false;
        if (nowAllSleeping) BroadcastNowSleepingMessage();
        ___serverChannel.BroadcastPacket(new NetworksMessageAllSleepMode { On = nowAllSleeping });
        __instance.AllSleeping = nowAllSleeping;
        return false;
    }

    private static void BroadcastNowSleepingMessage()
    {
        var serverMain = ApiEx.ServerMain;
        try
        {
            var allPlayers = serverMain.AllPlayersThatCouldSleep();
            IOC.Brighter.Send(new BroadcastMessageToAllPlayersCommand(
                messageCode: LangEx.FeatureCode("OneBedSleeping", "NowSleeping"),
                localiseForEachPlayer: true,
                args: string.Join(", ", allPlayers.SleepingPlayers().Select(p => p.PlayerName)))
            );
        }
        catch (ArgumentNullException e)
        {
            serverMain.Api.Logger.Error(e.Message);
            serverMain.Api.Logger.Error(e.StackTrace);
            throw;
        }
    }
}