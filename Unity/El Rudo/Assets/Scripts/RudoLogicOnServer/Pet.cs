using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;

[System.Serializable]
[CreateAssetMenu(fileName = "RudoLog", menuName = "ScriptableObjects/Pet", order = 2)]
public class Pet : PetStats {
    public Equipable Equipable;
    public Pet(int nftId, int equipableId, int quality) : base( PetsArray.GetInstance(equipableId).FighterName, PetsArray.GetInstance(equipableId).Vitality, PetsArray.GetInstance(equipableId).Strength, PetsArray.GetInstance(equipableId).Velocity, PetsArray.GetInstance(equipableId).Agility, new List<Weapon>(),null, PetsArray.GetInstance(equipableId).InitialDerivatedStats, PetsArray.GetInstance(equipableId).pathToAddressable)
    {
        Equipable = new Equipable(nftId,equipableId,quality);
    }
}
