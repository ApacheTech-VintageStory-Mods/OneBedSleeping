using Gantry.Core.Extensions.Api;

namespace ApacheTech.VintageMods.OneBedSleeping.Features.OneBedSleeping
{
    [UsedImplicitly]
    public sealed class OneBedSleeping : UniversalModSystem, IServerServiceRegistrar
    {
        private OneBedSleepingSettings _settings;

        public void ConfigureServerModServices(IServiceCollection services, ICoreServerAPI sapi)
        {
            sapi.Logger.GantryDebug("Adding OneBedSleeping feature settings to IOC.");
            services.AddFeatureWorldSettings<OneBedSleepingSettings>();
        }

        public override void StartServerSide(ICoreServerAPI sapi)
        {
            _settings = IOC.Services.Resolve<OneBedSleepingSettings>();
            var parsers = sapi.ChatCommands.Parsers;

            var command = sapi.ChatCommands.Create("obs")
                .RequiresPrivilege(Privilege.controlserver)
                .WithDescription(LangEntry("Command.Description"));

            command
                .BeginSubCommand("hunger")
                .WithDescription(LangEntry("Command.Hunger.Description"))
                .WithArgs(parsers.OptionalFloat("multiplier"))
                .HandleWith(OnHungerSubCommand)
                .EndSubCommand();

            command
                .BeginSubCommand("players")
                .WithDescription(LangEntry("Command.Players.Description"))
                .WithArgs(parsers.OptionalFloat("percentage"))
                .HandleWith(OnPercentSubCommand)
                .EndSubCommand();
        }

        private TextCommandResult OnHungerSubCommand(TextCommandCallingArgs args)
        {
            var value = args.Parsers[0].GetValue().To<float?>().GetValueOrDefault(1f);
            _settings.SaturationMultiplier = GameMath.Clamp(value, 0f, 2f);
            return TextCommandResult.Success(LangEntry("Command.Hunger", _settings.SaturationMultiplier * 100));
        }

        private TextCommandResult OnPercentSubCommand(TextCommandCallingArgs args)
        {
            var value = args.Parsers[0].GetValue().To<float?>().GetValueOrDefault(0f);
            _settings.PlayerThreshold = GameMath.Clamp(value / 100, 0f, 1f);
            return TextCommandResult.Success(LangEntry("Command.Players", _settings.PlayerThreshold * 100));
        }

        private static string LangEntry(string code, params object[] args)
            => LangEx.FeatureString("OneBedSleeping", code, args);

    }
}