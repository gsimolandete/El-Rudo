using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldStats
{
    //shieldHealth refers to the amount of damage a shield can take
    [SerializeField]
    public readonly float shieldHealth, blockRate, blockPercent;
    [SerializeField]
    public readonly string pathToAddressable;

    public ShieldStats(float shieldHealth, float blockRate, float blockPercent, string pathToAddressable)
    {
        this.shieldHealth = shieldHealth;
        this.blockRate = blockRate;
        this.blockPercent = blockPercent;
        this.pathToAddressable = pathToAddressable;
    }
}
