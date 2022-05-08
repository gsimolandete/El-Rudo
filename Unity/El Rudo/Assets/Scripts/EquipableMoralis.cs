using Moralis.Platform.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipableMoralis : MoralisObject
{
    public int nftId { get; set; }
    public string owner { get; set; }
    public int weaponId { get; set; }
    public int weaponQuality { get; set; }
}
