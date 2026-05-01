namespace TLK.Quoting;

/// <summary>
/// A drilling or boring operation. Cycle time accounts for both feed motion
/// and per-cycle peck retraction overhead.
/// </summary>
public interface IDrillCycleNode : ITlkNode
{
    /// <summary>Total drilled depth in inches.</summary>
    double DepthInches { get; }

    /// <summary>Feed rate in inches per spindle revolution.</summary>
    double FeedRateIpr { get; }

    /// <summary>
    /// Aggregate peck-retract overhead in seconds for this drill cycle
    /// (sum of all retract motions, not per-peck).
    /// </summary>
    double PeckRetractTimeSec { get; }

    /// <summary>Spindle speed in revolutions per minute.</summary>
    double SpindleRpm { get; }
}
