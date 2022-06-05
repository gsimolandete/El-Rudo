using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoadMiniRudoDisplay : MonoBehaviour
{
    [SerializeField]
    public TMP_Text name, nftId, elo, vitality, strength, agility, velocity, level;
    [SerializeField]
    Button buttonToRudoProfile;

    public void InitializeMini(Rudo rudo, CustomMainMenuScript customMainMenuScript, GameObject menu)
    {
        name.text = rudo.FighterName;
        elo.text = "0";
        vitality.text = rudo.Vitality.ToString();
        strength.text = rudo.Strength.ToString();
        agility.text = rudo.Agility.ToString();
        velocity.text = rudo.Velocity.ToString();
        level.text = rudo.Level.ToString();

        buttonToRudoProfile.onClick.AddListener( delegate { customMainMenuScript.NavigateTo(menu); menu.GetComponent<RudoViewerScript>().InitializeBig(rudo); });
    }
}
