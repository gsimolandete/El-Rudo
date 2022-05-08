using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FriendlyFightUIScript : MonoBehaviour
{
    [SerializeField]
    TMP_Dropdown Dropdown;
    [SerializeField]
    CustomMainMenuScript mainmenu;
    private async void OnEnable()
    {
        List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
        if(list == null)
        {
            await mainmenu.GetRudos();
        }

        foreach (var item in CustomMainMenuScript.ownRudos)
        {
            list.Add(new TMP_Dropdown.OptionData("RudoId: "+ item.NftId + " Name: "+ item.FighterName));
        }
        Dropdown.options = list;
    }
}
