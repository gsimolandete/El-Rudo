using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;

[System.Serializable]
[CreateAssetMenu(fileName = "RudoLog", menuName = "ScriptableObjects/Shield", order = 1)]
public class Shield : ShieldStats
{
    public Equipable Equipable;
    public Shield(int nftId, int equipableId, int quality) : base (ShieldArray.GetInstance(equipableId).shieldHealth, ShieldArray.GetInstance(equipableId).blockRate, ShieldArray.GetInstance(equipableId).blockPercent, ShieldArray.GetInstance(equipableId).pathToAddressable)
    {
        Equipable = new Equipable(nftId,equipableId,quality);
    }
}
