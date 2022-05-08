using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;

[System.Serializable]
[CreateAssetMenu(fileName = "RudoLog", menuName = "ScriptableObjects/Shield", order = 1)]
public class Shield : ScriptableObject
{
    public Equipable Equipable;
    public Shield(int nftId, int equipableId, int quality, string pathToAddressable)
    {
        Equipable = new Equipable(nftId,equipableId,quality, pathToAddressable);
    }

    //shieldHealth refers to the amount of damage a shield can take
    [SerializeField]
    readonly float shieldHealth, vitality, strength, agility, velocity, blockRate, parryRate, blockPercent;

    public float Vitality => vitality;

    public float Strength => strength;

    public float Agility => agility;

    public float Velocity => velocity;

    public float BlockRate => blockRate;

    public float ParryRate => parryRate;

    public float BlockPercent => blockPercent;

    public float ShieldHealth => shieldHealth;
}
