using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "RudoLog", menuName = "ScriptableObjects/Pet", order = 2)]
public class Pet : Fighter {
    public Equipable Equipable;
    public Pet(int nftId, int equipableId, int quality, string pathToAddressable, string name, float vitality, float strength, float velocity, float agility) : base( name, vitality, strength, velocity, agility, new List<Weapon>(),null)
    {
        Equipable = new Equipable(nftId,equipableId,quality, pathToAddressable);
    }
}
