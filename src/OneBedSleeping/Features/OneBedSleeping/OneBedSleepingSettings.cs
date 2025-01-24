using Gantry.Services.FileSystem.Configuration.Abstractions;

namespace ApacheTech.VintageMods.OneBedSleeping.Features.OneBedSleeping;

/// <summary>
///     Represents the settings for the one-bed sleeping feature, allowing for configuration of saturation
///     and player thresholds.
/// </summary>
/// <remarks>
///     This class is a feature-specific settings implementation that defines customisable parameters
///     for the one-bed sleeping functionality. It inherits from <see cref="FeatureSettings{T}"/>.
/// </remarks>
public sealed class OneBedSleepingSettings : FeatureSettings<OneBedSleepingSettings>
{
    /// <summary>
    ///     Multiplier applied to player saturation during one-bed sleeping.
    /// </summary>
    /// <remarks>
    ///     This value determines how the player's saturation is affected when utilising the one-bed sleeping feature.
    ///     A value of <c>1f</c> applies no change, while values greater or less than 1 scale the saturation accordingly.
    /// </remarks>
    public float SaturationMultiplier { get; set; } = 1f;

    /// <summary>
    ///     Minimum threshold of players required to enable the one-bed sleeping feature.
    /// </summary>
    /// <remarks>
    ///     The <c>PlayerThreshold</c> determines the fraction of players required for the feature to activate.
    ///     A value of <c>0f</c> indicates no threshold, whereas higher values increase the requirement proportionally.
    /// </remarks>
    public float PlayerThreshold { get; set; } = 0f;
}