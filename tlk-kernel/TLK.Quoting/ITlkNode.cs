namespace TLK.Quoting;

/// <summary>
/// Base contract for any node in a TLK timeline. Concrete nodes (turning,
/// drilling, sync, etc.) extend this interface and add their own parameters.
/// </summary>
/// <remarks>
/// Consumers should pattern-match on the concrete sub-interface
/// (<see cref="ITurnCycleNode"/>, <see cref="IDrillCycleNode"/>,
/// <see cref="ISyncNode"/>, …) and treat unknown types as no-ops so new
/// node kinds can be added without breaking older consumers.
/// </remarks>
public interface ITlkNode
{
    /// <summary>
    /// Discriminator string for this node kind (e.g. "turn_cycle", "drill_cycle", "sync").
    /// Mirrors the JSON <c>node_type</c> field; consumers normally rely on the
    /// runtime sub-interface rather than this string.
    /// </summary>
    string NodeType { get; }
}
