using SatinRoad.Core.Orders;

namespace SatinRoad.Tests.Orders;

/// <summary>Always returns the given result and remembers which probability was asked for.</summary>
public class FakeChanceProvider(bool result) : IChanceProvider
{
    public double? LastProbability { get; private set; }
    public int Rolls { get; private set; }

    public bool Roll(double probability)
    {
        LastProbability = probability;
        Rolls++;
        return result;
    }
}
