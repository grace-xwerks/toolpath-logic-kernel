namespace TLK.Quoting;

/// <summary>
/// A multi-channel synchronization point. Each channel runs its own sub-timeline
/// in parallel; total elapsed time for the sync block is the longest channel.
/// </summary>
/// <remarks>
/// Sync nodes can be nested — a channel's timeline may contain its own sync nodes.
/// Cycle-time estimators should recurse.
/// </remarks>
public interface ISyncNode : ITlkNode
{
    /// <summary>
    /// Per-channel sub-timelines. Outer list = channels, inner list = the operations
    /// that channel executes within this sync block.
    /// </summary>
    IReadOnlyList<IReadOnlyList<ITlkNode>> Channels { get; }
}
