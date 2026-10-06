using SatinRoad.Core.Orders;

namespace SatinRoad.Tests.Orders;

// The real random generator – only the deterministic edge cases can be tested
public class RandomChanceProviderTests
{
    private readonly RandomChanceProvider _chance = new();

    [Fact]
    public void Roll_ZeroProbability_IsNeverTrue()
    {
        for (var i = 0; i < 1000; i++)
            Assert.False(_chance.Roll(0.0));
    }

    [Fact]
    public void Roll_FullProbability_IsAlwaysTrue()
    {
        for (var i = 0; i < 1000; i++)
            Assert.True(_chance.Roll(1.0));
    }
}
