namespace TLK.Quoting;

/// <summary>
/// Root contract for a TLK job — the shape consumed by the quoting engine
/// (and any other downstream tool that reads validated TLK output).
/// </summary>
public interface ITlkJobJson
{
    /// <summary>Machine setup for the job (type, stock geometry, channel count).</summary>
    IMachineNode Machine { get; }

    /// <summary>
    /// Ordered timeline of operations. Sync nodes carry their own per-channel
    /// sub-timelines; everything else is sequential at this level.
    /// </summary>
    IReadOnlyList<ITlkNode> Timeline { get; }

    /// <summary>
    /// Material code (e.g. "304ss", "6061al"). Must match a key in the consumer's
    /// rate table or the consumer should treat the job as unquotable.
    /// </summary>
    string Material { get; }
}
