// ReSharper disable InconsistentNaming
// ReSharper disable RedundantAssignment

namespace ApacheTech.VintageMods.OneBedSleeping.Features.OneBedSleeping.Patches
{
    [SettingsConsumer(EnumAppSide.Server)]
    [HarmonySidedPatch(EnumAppSide.Server)]
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
        public static bool Patch_ModSleeping_ServerSlowTick_Prefix(ModSleeping __instance, 
            ICoreServerAPI ___sapi, ref double ___lastTickTotalDays, IServerNetworkChannel ___serverChannel)
        {
            var value = Settings.SaturationMultiplier;
            var flag = __instance.AreAllPlayersSleeping();
            if (flag)
            {
                if (__instance.AllSleeping)
                {
                    foreach (var player in ___sapi.World.AllOnlinePlayers)
                    {
                        var behaviour = (player as IServerPlayer)?.Entity.GetBehavior<EntityBehaviorHunger>();
                        if (behaviour is null) continue;
                        var saturation = (float)(___sapi.World.Calendar.TotalDays - ___lastTickTotalDays) * (2000f * value);
                        behaviour.ConsumeSaturation(saturation);
                    }
                }
                ___lastTickTotalDays = ___sapi.World.Calendar.TotalDays;
            }

            if (flag == __instance.AllSleeping) return false;
            if (flag) BroadcastNowSleepingMessage();
            ___serverChannel.BroadcastPacket(new NetworksMessageAllSleepMode { On = flag });
            __instance.AllSleeping = flag;
            return false;
        }

        private static void BroadcastNowSleepingMessage()
        {
            var serverMain = ApiEx.ServerMain;
            try
            {
                var allPlayers = serverMain.AllPlayersThatCouldSleep().ToList();
                var sleepingPlayers = allPlayers.SleepingPlayers().Select(p => p.PlayerName).ToList();
                var messageCode = LangEx.FeatureCode("OneBedSleeping", "NowSleeping");
         
                foreach (var player in allPlayers)
                {
                    var message = Lang.GetL(player.LanguageCode, messageCode, string.Join(", ", sleepingPlayers));
                    serverMain.SendMessage(player, GlobalConstants.AllChatGroups, message, EnumChatType.Notification);
                }
            }
            catch (ArgumentNullException e)
            {
                serverMain.Api.Logger.Error(e.Message);
                serverMain.Api.Logger.Error(e.StackTrace);
                throw;
            }
        }
    }
}