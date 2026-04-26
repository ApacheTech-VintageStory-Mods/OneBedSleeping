namespace OneBedSleeping.Features.OneBedSleeping.Extensions;

public static class PlayerSleepExtensions
{
    public static IEnumerable<IServerPlayer> AllPlayersThatCouldSleep(this IServerWorldAccessor world)
    {
        return world.AllOnlinePlayers
            .Cast<IServerPlayer>()
            .Where(p =>
                p?.ConnectionState is EnumClientState.Playing &&
                p.WorldData.CurrentGameMode is not EnumGameMode.Spectator);
    }

    public static IEnumerable<IServerPlayer> PlayersInBed(this IServerWorldAccessor world)
    {
        return world
            .AllPlayersThatCouldSleep()
            .Where(p => p.Entity?.MountedOn is BlockEntityBed);
    }

    public static IEnumerable<IServerPlayer> PlayersNotInBed(this IServerWorldAccessor world)
    {
        return world
            .AllPlayersThatCouldSleep()
            .Where(p => p.Entity?.MountedOn is not BlockEntityBed);
    }
}