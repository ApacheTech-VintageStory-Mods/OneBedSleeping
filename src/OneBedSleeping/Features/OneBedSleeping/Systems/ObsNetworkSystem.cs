using OneBedSleeping.Features.OneBedSleeping.Settings;

namespace OneBedSleeping.Features.OneBedSleeping.Systems;

public sealed class ObsNetworkSystem : UniversalModSystem<ObsNetworkSystem>
{
    private OneBedSleepingSettings? _settings;
    private IServerNetworkChannel? _serverChannel;

    internal bool SleepingAllowed { get; set; } = true;

    public override void StartServerSide(ICoreServerAPI api)
    {
        _serverChannel = api.Network.GetOrRegisterDefaultChannel(Core)
            .RegisterMessageType<ObsSleepState>();

        _settings = Core.Resolve<OneBedSleepingSettings>();
        SleepingAllowed = _settings.AllowSleeping;
        _settings.AddPropertyChangedAction(p => p.AllowSleeping, Event_AllowSleepingChanged);

        api.Event.PlayerJoin += Event_PlayerJoin;
    }

    private void Event_AllowSleepingChanged(bool state)
    {
        if (_serverChannel is null)
        {
            Core.Logger.Error("Server network channel not initialised.");
            return;
        }
        _serverChannel.BroadcastPacket(new ObsSleepState
        {
            AllowSleeping = state
        });
    }

    private void Event_PlayerJoin(IServerPlayer byPlayer)
    {
        if (_serverChannel is null)
        {
            Core.Logger.Error("Server network channel not initialised.");
            return;
        }

        if (_settings is null)
        {
            Core.Logger.Error("Settings not initialised.");
            return;
        }

        _serverChannel.SendPacket(new ObsSleepState
        {
            AllowSleeping = _settings.AllowSleeping
        }, byPlayer);
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        api.Network.GetOrRegisterDefaultChannel(Core)
            .RegisterMessageType<ObsSleepState>()
            .SetMessageHandler<ObsSleepState>(OnObsSleepStateResponse);
    }

    private void OnObsSleepStateResponse(ObsSleepState packet) 
        => SleepingAllowed = packet.AllowSleeping;

    [ProtoContract]
    public class ObsSleepState
    {
        [ProtoMember(2)]
        public bool AllowSleeping { get; set; }
    }

    public override void Dispose()
    {
        if (G.Side.IsClient()) return;
        Sapi.Event.PlayerJoin -= Event_PlayerJoin;
    }
}