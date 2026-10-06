namespace SatinRoad.Core.Orders;

/// <summary>Source of randomness behind an interface, so tests can decide the outcome.</summary>
public interface IChanceProvider
{
    /// <summary>Returns true with the given probability (0.0 – 1.0).</summary>
    bool Roll(double probability);
}
