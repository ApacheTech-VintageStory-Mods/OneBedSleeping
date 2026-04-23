using Gantry.Services.Mediator.Chat.Commands;
using OneBedSleeping.Settings;

namespace OneBedSleeping.Patches;

public sealed class ModSleepingServerPatches : GantrySettingsPatch<OneBedSleepingSettings>
{
    [HarmonyPrefix]
    [HarmonyServerPatch(typeof(ModSleeping), "AreAllPlayersSleeping")]
    public static bool Patch_ModSleeping_AreAllPlayersSleeping_Prefix(ICoreServerAPI ___sapi, ref bool __result)
    {
        var allPlayers = ___sapi.World.AllPlayersThatCouldSleep().ToList();
        var playersSleeping = allPlayers.SleepingPlayers().Count();
        var requiredNumberOfPlayers = GameMath.Clamp(Math.Ceiling(allPlayers.Count * Settings.PlayerThreshold), 1, allPlayers.Count);
        __result = playersSleeping >= requiredNumberOfPlayers;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyServerPatch(typeof(ModSleeping), "ServerSlowTick")]
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
        var serverMain = Gantry.ApiEx.ServerMain;
        try
        {
            var allPlayers = serverMain.AllPlayersThatCouldSleep();
            Gantry.CommandProcessor.Execute(new BroadcastMessageToAllPlayersCommand(
                messageCode: Gantry.Lang.Code("OneBedSleeping", "NowSleeping"),
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