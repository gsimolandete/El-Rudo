using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadMiniRudoDisplay : MonoBehaviour
{
    [SerializeField]
    TMP_Text name, elo, vitality, strength, agility, velocity;

    public void Initialize(Rudo abstractRudo)
    {
        name.text = abstractRudo.FighterName;
        elo.text = "0";
        vitality.text = abstractRudo.Vitality.ToString();
        strength.text = abstractRudo.Strength.ToString();
        agility.text = abstractRudo.Agility.ToString();
        velocity.text = abstractRudo.Velocity.ToString();
    }
}
