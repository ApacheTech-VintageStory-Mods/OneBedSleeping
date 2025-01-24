using System.Collections.Generic;

namespace ApacheTech.VintageMods.OneBedSleeping.Features.OneBedSleeping.Patches;

/// <summary>
///     Provides extension methods for server-side world and player-related operations.
/// </summary>
public static class ServerWorldExtensions
{
    /// <summary>
    ///     Retrieves all online players in the world who are eligible to sleep.
    /// </summary>
    /// <param name="world">The server world accessor.</param>
    /// <returns>
    ///     A collection of server players who are currently online, not in spectator mode, and in a playable state.
    /// </returns>
    /// <remarks>
    ///     This method filters out players who are not actively playing or are in spectator mode.
    /// </remarks>
    public static IEnumerable<IServerPlayer> AllPlayersThatCouldSleep(this IServerWorldAccessor world)
        => world.AllOnlinePlayers
            .Cast<IServerPlayer>()
            .Where(p => p?.ConnectionState == EnumClientState.Playing)
            .Where(p => p.WorldData.CurrentGameMode != EnumGameMode.Spectator);

    /// <summary>
    ///     Filters a collection of players to retrieve only those who are currently sleeping in beds.
    /// </summary>
    /// <param name="players">The collection of server players.</param>
    /// <returns>A collection of server players who are currently sleeping in beds.</returns>
    /// <remarks>
    ///     A player is considered sleeping if their entity is mounted on a bed block entity.
    /// </remarks>
    public static IEnumerable<IServerPlayer> SleepingPlayers(this IEnumerable<IServerPlayer> players)
        => players.Where(p => p.Entity?.MountedOn is BlockEntityBed);

    /// <summary>
    ///     Toggles the value of a <see cref="bool"/> between <c>true</c> and <c>false</c>.
    /// </summary>
    /// <param name="value">The reference to the boolean value to toggle.</param>
    /// <returns>The toggled value of the boolean.</returns>
    public static bool Toggle(this ref bool value) => value = !value;

    /// <summary>
    ///     Ensures that a <see cref="bool"/> value is set to <c>true</c>.
    /// </summary>
    /// <param name="value">The reference to the boolean value to modify.</param>
    /// <returns>The updated boolean value, which will always be <c>true</c>.</returns>
    public static bool EnsureTrue(this ref bool value) => value = true;

    /// <summary>
    ///     Ensures that a <see cref="bool"/> value is set to <c>false</c>.
    /// </summary>
    /// <param name="value">The reference to the boolean value to modify.</param>
    /// <returns>The updated boolean value, which will always be <c>false</c>.</returns>
    public static bool EnsureFalse(this ref bool value) => value = false;
}