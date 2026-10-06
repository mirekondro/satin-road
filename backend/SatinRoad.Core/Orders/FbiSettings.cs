namespace SatinRoad.Core.Orders;

/// <summary>Hard story #12 – probability that a buyer is FBI (0.01 = 1%). Configurable for the demo.</summary>
public record FbiSettings(double Chance = 0.01);
