namespace SatinRoad.Core.Orders;

public class RandomChanceProvider : IChanceProvider
{
    // NextDouble() returns 0.0 – 0.99999…, so probability 0 is never true and 1 is always true
    public bool Roll(double probability) => Random.Shared.NextDouble() < probability;
}
