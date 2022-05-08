using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FriendlyFightUIScript : MonoBehaviour
{
    [SerializeField]
    TMP_Dropdown Dropdown;
    [SerializeField]
    TMP_InputField InputField;
    [SerializeField]
    CustomMainMenuScript mainmenu;
    [SerializeField]
    LoadingScenesScript LoadingScenesScript;
    private async void OnEnable()
    {
        List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
        if(CustomMainMenuScript.ownRudos == null)
        {
            await mainmenu.GetRudos();
        }

        foreach (var item in CustomMainMenuScript.ownRudos)
        {
            list.Add(new TMP_Dropdown.OptionData("RudoId: "+ item.NftId + " Name: "+ item.FighterName));
        }
        Dropdown.options = list;
    }

    public async void LoadFight()
    {
        CombatDynamics.rudo1 = CustomMainMenuScript.ownRudos[Dropdown.value];
        CombatDynamics.rudo2 = await mainmenu.GetRudo(int.Parse(InputField.text));
        LoadingScenesScript.LoadFightingScene();
    }
}
