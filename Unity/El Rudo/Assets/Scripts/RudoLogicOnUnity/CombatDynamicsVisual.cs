using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using static FighterCombat;
using TMPro;

public class CombatDynamicsVisual : CombatDynamics
{
    [SerializeField]
    private Transform team1Spawn, team2Spawn;
    [SerializeField]
    private AssetReference RudoPrefab;
    [SerializeField]
    Transform CombatGameobjects;
    [SerializeField]
    Slider hpSlider1, hpSlider2, weaponSlider1, weaponSlider2;
    [SerializeField]
    TMP_Text rudoNameText1, rudoNameText2;
    [SerializeField]
    public GameObject LoadMainMenu;
    public CombatDynamicsVisual(bool randomSeed, Rudo abstractRudo1, Rudo abstractRudo2, int seed = 0) : base( randomSeed,  abstractRudo1,  abstractRudo2,  seed)
    {
    }

    private void Start()
    {
        StartCombat();
    }

    protected override async Task PrepareFight()
    {
        await base.PrepareFight();

        for (int i = 0; i < CombatGameobjects.childCount; i++)
        {
            Destroy(CombatGameobjects.GetChild(i).gameObject);
        }

        GameObject go = await RudoPrefab.InstantiateAsync(team1Spawn.position, Quaternion.identity, CombatGameobjects).Task;
        (teamFighterList[0].Rudo as RudoCombatVisual).InitializeRudoCombatVisual(go, team1Spawn.position, team2Spawn.position);
        rudoNameText1.text = teamFighterList[0].Rudo.Fighter.FighterName;

        if (teamFighterList[0].pet != null)
        {
            Vector3 petspawn = team1Spawn.position+new Vector3(1,1,0);
            GameObject goPet = await Addressables.InstantiateAsync((teamFighterList[0].pet.Fighter as Pet).pathToAddressable, petspawn, Quaternion.identity, CombatGameobjects).Task;
            (teamFighterList[0].pet as PetCombatVisual).InitializeRudoCombatVisual(goPet, petspawn, team2Spawn.position);
            rudoNameText1.text = teamFighterList[0].pet.Fighter.FighterName;
        }

        GameObject go1 = await RudoPrefab.InstantiateAsync(team2Spawn.position, Quaternion.identity, CombatGameobjects).Task;
        (teamFighterList[1].Rudo as RudoCombatVisual).InitializeRudoCombatVisual(go1, team2Spawn.position, team1Spawn.position);
        rudoNameText2.text = teamFighterList[1].Rudo.Fighter.FighterName;

        if (teamFighterList[1].pet != null)
        {
            Vector3 petspawn = team2Spawn.position + new Vector3(-1, 1, 0);
            GameObject goPet = await Addressables.InstantiateAsync((teamFighterList[1].pet.Fighter as Pet).pathToAddressable, petspawn, Quaternion.identity, CombatGameobjects).Task;
            (teamFighterList[1].pet as PetCombatVisual).InitializeRudoCombatVisual(goPet, petspawn, team1Spawn.position);
            rudoNameText2.text = teamFighterList[0].pet.Fighter.FighterName;
        }
    }

    protected override void GetTeams()
    {
        teamFighterList[0] = rudo1.GetTeamVisual(TeamNum.Team1, hpSlider1, weaponSlider1);
        teamFighterList[1] = rudo2.GetTeamVisual(TeamNum.Team2, hpSlider2, weaponSlider2);
    }
    protected override void OnCombatEnded()
    {
        LoadMainMenu.SetActive(true);
    }
}
