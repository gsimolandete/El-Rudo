using System.Collections;
using System.Collections.Generic;

public class ShieldStats
{
    //shieldHealth refers to the amount of damage a shield can take
    public readonly float shieldHealth, blockRate, blockPercent;
    public readonly string pathToAddressable;

    public ShieldStats(float shieldHealth, float blockRate, float blockPercent, string pathToAddressable)
    {
        this.shieldHealth = shieldHealth;
        this.blockRate = blockRate;
        this.blockPercent = blockPercent;
        this.pathToAddressable = pathToAddressable;
    }
}
