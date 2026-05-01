namespace TLK.Quoting;

/// <summary>
/// Machine setup for a TLK job — the immutable physical context the operations run against.
/// </summary>
public interface IMachineNode
{
    /// <summary>
    /// Machine archetype: "swiss", "lathe", "mill", etc. Consumers map this string
    /// to setup-time and rate tables.
    /// </summary>
    string MachineType { get; }

    /// <summary>Bar / billet outside diameter in inches.</summary>
    double StockDiameterInches { get; }

    /// <summary>Cut length per piece in inches (the slug consumed off the bar).</summary>
    double StockLengthInches { get; }

    /// <summary>
    /// Number of independently-controlled channels. Used to detect multi-channel
    /// work and cap parallelism in cycle-time estimation.
    /// </summary>
    int ChannelCount { get; }
}
