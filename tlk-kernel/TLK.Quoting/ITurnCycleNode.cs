namespace TLK.Quoting;

/// <summary>
/// A turning (or facing) operation against the OD/ID of the stock.
/// Cycle time is derived from depth-of-cut, feed rate, pass count, and spindle RPM.
/// </summary>
public interface ITurnCycleNode : ITlkNode
{
    /// <summary>Material removed per pass, in inches.</summary>
    double DepthOfCutInches { get; }

    /// <summary>Feed rate in inches per spindle revolution.</summary>
    double FeedRateIpr { get; }

    /// <summary>Number of cutting passes for this operation.</summary>
    int Passes { get; }

    /// <summary>Spindle speed in revolutions per minute.</summary>
    double SpindleRpm { get; }
}
