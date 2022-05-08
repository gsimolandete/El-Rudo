using Moralis.Platform.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RudoMoralis : MoralisObject
{
    public int rudoId { get; set; }
    public string name { get; set; }
    public int level { get; set; }
    public int vitality { get; set; }
    public int strength { get; set; }
    public int agility { get; set; }
    public int velocity { get; set; }
    public int elo { get; set; }
    public int experience { get; set; }
    public PetMoralis pet { get; set; }
    public ShieldMoralis shield { get; set; }
    public List<WeaponMoralis> weapons { get; set; }
    public string owner { get; set; }
}
