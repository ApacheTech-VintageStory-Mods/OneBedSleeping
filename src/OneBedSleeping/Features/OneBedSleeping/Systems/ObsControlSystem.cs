using OneBedSleeping.Features.OneBedSleeping.Extensions;
using OneBedSleeping.Features.OneBedSleeping.Settings;

namespace OneBedSleeping.Features.OneBedSleeping.Systems;

public sealed class ObsControlSystem : ServerModSystem<ObsControlSystem>
{
    private ICoreServerAPI _sapi = null!;
    private OneBedSleepingSettings _settings = null!;

    private double _lastSleepHours;
    private bool _isSleepActive;

    public bool IsSleepActive => _isSleepActive;

    public override void StartServerSide(ICoreServerAPI sapi)
    {
        _sapi = sapi;
        _settings = Core.Services.GetRequiredService<OneBedSleepingSettings>();

        sapi.Event.RegisterGameTickListener(OnSleepTick, 200);
    }

    private void OnSleepTick(float dt)
    {
        var nowSleeping = AreEnoughPlayersSleeping();

        if (nowSleeping && !_isSleepActive)
        {
            _lastSleepHours = _sapi.World.Calendar.TotalHours;
        }

        _isSleepActive = nowSleeping;

        if (!_isSleepActive) return;

        var now = _sapi.World.Calendar.TotalHours;
        var hoursPassed = now - _lastSleepHours;

        if (hoursPassed <= 0) return;

        _lastSleepHours = now;

        var sleepEfficiency = GetSleepEfficiency();

        foreach (var player in _sapi.World.PlayersNotInBed())
        {
            RestPlayer(player, sleepEfficiency, hoursPassed);
        }
    }

    private bool AreEnoughPlayersSleeping()
    {
        if (!_settings.AllowSleeping) return false;

        var players = _sapi.World.AllPlayersThatCouldSleep().ToList();

        if (players.Count is 0) return false;

        var sleeping = players.Count(p => p.Entity.GetBehavior<EntityBehaviorTiredness>()!.IsSleeping);

        var required = Math.Max(1, (int)Math.Ceiling(players.Count * _settings.PlayerThreshold));

        return sleeping >= required;
    }

    private float GetSleepEfficiency()
    {
        return _sapi.World.PlayersInBed()
            .Select(p => p.Entity.MountedOn.To<BlockEntityBed>().GetField<float>("sleepEfficiency"))
            .DefaultIfEmpty(0.5f)
            .Min();
    }

    private void RestPlayer(IServerPlayer player, float sleepEfficiency, double hoursPassed)
    {
        var sleepEff = sleepEfficiency - 1f / 12f;
        if (sleepEff <= 0f) return;

        if (_sapi.World.Config.GetString("temporalStormSleeping", "0").ToInt(0) == 0 &&
            _sapi.ModLoader.GetModSystem<SystemTemporalStability>(true).StormStrength > 0f)
        {
            return;
        }

        var ebt = player.Entity.GetBehaviour<EntityBehaviorTiredness>("tiredness");
        if (ebt is null) return;

        ebt.Tiredness = Math.Max(0f, ebt.Tiredness - (float)hoursPassed / sleepEff);
    }
}