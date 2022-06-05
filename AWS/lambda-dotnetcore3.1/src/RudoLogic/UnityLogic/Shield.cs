using System.Collections;
using System.Collections.Generic;
using static GlobalVariables;

[System.Serializable]
public class Shield : ShieldStats
{
    public Equipable Equipable;
    public Shield(int nftId, int equipableId, int quality) : base (ShieldArray.GetInstance(equipableId).shieldHealth, ShieldArray.GetInstance(equipableId).blockRate, ShieldArray.GetInstance(equipableId).blockPercent, ShieldArray.GetInstance(equipableId).pathToAddressable)
    {
        Equipable = new Equipable(nftId,equipableId,quality);
    }
}
