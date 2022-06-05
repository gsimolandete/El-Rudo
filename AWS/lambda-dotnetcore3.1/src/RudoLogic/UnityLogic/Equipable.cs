using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Equipable
{
    public int nftId;
    public int equipableId;
    public int quality;

    public Equipable(int nftId, int equipableId, int quality)
    {
        this.nftId = nftId;
        this.equipableId = equipableId;
        this.quality = quality;
    }
}
