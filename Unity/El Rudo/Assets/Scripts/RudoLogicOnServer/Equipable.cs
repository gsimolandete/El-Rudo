using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Equipable
{
    public int nftId;
    public int equipableId;
    public int quality;
    public string pathToAddressable;

    public Equipable(int nftId, int equipableId, int quality, string pathToAddressable)
    {
        this.nftId = nftId;
        this.equipableId = equipableId;
        this.quality = quality;
        this.pathToAddressable = pathToAddressable;
    }
}
