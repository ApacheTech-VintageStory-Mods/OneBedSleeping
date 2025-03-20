using System.Text;
using Gantry.Core.Extensions.Api;

namespace ApacheTech.VintageMods.OneBedSleeping.Features.OneBedSleeping;

[UsedImplicitly]
public sealed class OneBedSleeping : UniversalModSystem, IServerServiceRegistrar
{
    private OneBedSleepingSettings _settings;

    public void ConfigureServerModServices(IServiceCollection services, ICoreServerAPI sapi)
    {
        G.Log("Adding OneBedSleeping feature settings to IOC.");
        services.AddFeatureWorldSettings<OneBedSleepingSettings>();
    }

    public override void StartServerSide(ICoreServerAPI sapi)
    {
        _settings = IOC.Services.Resolve<OneBedSleepingSettings>();
        var parsers = sapi.ChatCommands.Parsers;

        var command = sapi.ChatCommands.Create("obs")
            .RequiresPrivilege(Privilege.controlserver)
            .WithDescription(T("Command.Description"))
            .HandleWith(DisplaySettings);

        command
            .BeginSubCommand("hunger")
            .WithDescription(T("Command.Hunger.Description"))
            .WithArgs(parsers.OptionalFloat("multiplier"))
            .HandleWith(OnHungerSubCommand)
            .EndSubCommand();

        command
            .BeginSubCommand("players")
            .WithDescription(T("Command.Players.Description"))
            .WithArgs(parsers.OptionalFloat("percentage"))
            .HandleWith(OnPercentSubCommand)
            .EndSubCommand();
    }

    private TextCommandResult DisplaySettings(TextCommandCallingArgs args)
    {
        var sb = new StringBuilder();
        sb.AppendLine(T("Command.Players", _settings.PlayerThreshold * 100));
        sb.AppendLine(T("Command.Hunger", _settings.SaturationMultiplier * 100));
        return TextCommandResult.Success(sb.ToString());
    }

    private TextCommandResult OnHungerSubCommand(TextCommandCallingArgs args)
    {
        var value = args.Parsers[0].GetValue().To<float?>().GetValueOrDefault(1f);
        _settings.SaturationMultiplier = GameMath.Clamp(value, 0f, 2f);
        return TextCommandResult.Success(T("Command.Hunger", _settings.SaturationMultiplier * 100));
    }

    private TextCommandResult OnPercentSubCommand(TextCommandCallingArgs args)
    {
        var value = args.Parsers[0].GetValue().To<float?>().GetValueOrDefault(0f);
        _settings.PlayerThreshold = GameMath.Clamp(value / 100, 0f, 1f);
        return TextCommandResult.Success(T("Command.Players", _settings.PlayerThreshold * 100));
    }

    private static string T(string code, params object[] args)
        => LangEx.FeatureString("OneBedSleeping", code, args);
}