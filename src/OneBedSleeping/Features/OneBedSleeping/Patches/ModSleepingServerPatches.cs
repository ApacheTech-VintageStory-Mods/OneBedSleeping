using Gantry.Services.Mediator.Chat.Commands;
using OneBedSleeping.Features.OneBedSleeping.Extensions;
using OneBedSleeping.Features.OneBedSleeping.Settings;
using OneBedSleeping.Features.OneBedSleeping.Systems;

namespace OneBedSleeping.Features.OneBedSleeping.Patches;

public sealed class ModSleepingServerPatches : GantrySettingsPatch<OneBedSleepingSettings>
{
    [HarmonyPrefix]
    [HarmonyServerPatch(typeof(ModSleeping), "ServerSlowTick")]
    public static bool Patch_ModSleeping_ServerSlowTick_Prefix(ModSleeping __instance, IServerNetworkChannel ___serverChannel)
    {
        Debug.Assert(G.Side is EnumAppSide.Server);

        var system = G.Sapi.ModLoader.GetModSystem<ObsControlSystem>();

        var nowSleeping = system.IsSleepActive;

        if (nowSleeping == __instance.AllSleeping)
        {
            return false;
        }

        if (nowSleeping)
        {
            BroadcastNowSleepingMessage();
        }

        ___serverChannel.BroadcastPacket(new NetworksMessageAllSleepMode { On = nowSleeping });

        __instance.AllSleeping = nowSleeping;

        return false;
    }

    private static void BroadcastNowSleepingMessage()
    {
        Debug.Assert(G.Side is EnumAppSide.Server);
        var serverMain = Gantry.ApiEx.ServerMain;
        try
        {
            var allPlayers = serverMain.AllPlayersThatCouldSleep();
            Gantry.CommandProcessor.Execute(new BroadcastMessageToAllPlayersCommand(
                messageCode: Gantry.Lang.Code("OneBedSleeping", "NowSleeping"),
                localiseForEachPlayer: true,
                args: string.Join(", ", serverMain.PlayersInBed().Select(p => p.PlayerName)))
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