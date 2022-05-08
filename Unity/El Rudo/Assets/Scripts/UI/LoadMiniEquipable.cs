using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadMiniEquipable : MonoBehaviour
{
    [SerializeField]
    public TMP_Text nftId,equipableId,quality;
    [SerializeField]
    public Image image;
    [SerializeField]
    public Sprite sword, shield, pet;

    public void Initialize(EquipableMoralis equipable)
    {
        switch (equipable)
        {
            case PetMoralis e:
                image.sprite = pet;
                break;
            case WeaponMoralis e:
                image.sprite = sword;
                break;
            case ShieldMoralis e:
                image.sprite = shield;
                break;
            default:
                break;
        }
        nftId.text = equipable.nftId.ToString();
        quality.text = equipable.weaponQuality.ToString();
        equipableId.text = equipable.weaponId.ToString();
    }
}
