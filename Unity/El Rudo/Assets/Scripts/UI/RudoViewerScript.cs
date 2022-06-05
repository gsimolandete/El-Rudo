using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using static GlobalVariables;
using MoralisUnity;
using MoralisUnity.Web3Api.Models;
using Nethereum.Hex.HexTypes;
using System;
using Newtonsoft.Json.Linq;

public class RudoViewerScript : LoadMiniRudoDisplay
{
    [SerializeField]
    GameObject weaponGridLayoutGroup, skillsGridLayoutGroup, petButton, shieldButton, EquipableButton;
    [SerializeField]
    Sprite sword, shield, pet, empty;
    [SerializeField]
    TMP_InputField expInput;
    [SerializeField]
    CustomMainMenuScript CustomMainMenuScript;
    [SerializeField]
    Slider expSlider;
    [SerializeField]
    List<TMP_Text> levelUpButtons;

    int setType;
    int selectedButton;

    Rudo rudoDisplayed;
    public async void SetLevelUp()
    {
        // Function ABI input parameters
        object[] inputParams = new object[1];
        inputParams[0] = new { internalType = "uint256", name = "id", type = "uint256" };
        // Function ABI Output parameters
        object[] outputParams = new object[1];
        outputParams[0] = new
        {
            components = new object[2] { new { internalType  = "enum Rudo.LvlIncType", name = "choiseType", type = "uint8" },
                                                            new { internalType = "uint16[4]", name = "statsIncrease", type = "uint16[4]" } },
            internalType = "struct Rudo.LevelIncreaseChoise[3]",
            name = "",
            type = "tuple[3]"
        };
        // Function ABI
        object[] abi = new object[1];
        abi[0] = new { inputs = inputParams, name = "GetLevelIncreases", outputs = outputParams, stateMutability = "view", type = "function" };
        // Define request object
        RunContractDto rcd = new RunContractDto()
        {
            Abi = abi,
            Params = new { id = rudoDisplayed.NftId }
        };
        List<List<object>> s = await ContractCalls.RunContractFunction<List<List<object>>>(RUDO_CONTRACT_ADDRESS, "GetLevelIncreases", rcd, ChainList.mumbai);

        KeyValuePair<int, int[]>[] kvps = new KeyValuePair<int, int[]>[3];
        for (int i = 0; i < 3; i++)
        {
            int a = int.Parse((string)s[i][0]);
            JArray b = s[i][1] as JArray;
            kvps[i] = new KeyValuePair<int, int[]>(a, new int[4] { b[0].ToObject<int>(), b[1].ToObject<int>(), b[2].ToObject<int>(), b[3].ToObject<int>() });
        }

        levelUpButtons[0].text = kvps[0].Key == 1 ? "vitality: " + kvps[0].Value[0] + " strength: " + kvps[0].Value[1] + " agility: " + kvps[0].Value[2] + " velocity: " + kvps[0].Value[3] : SkillsArray.GetInstance(kvps[0].Value[0]).name;
        levelUpButtons[1].text = kvps[1].Key == 1 ? "vitality: " + kvps[1].Value[0] + " strength: " + kvps[1].Value[1] + " agility: " + kvps[1].Value[2] + " velocity: " + kvps[1].Value[3] : SkillsArray.GetInstance(kvps[1].Value[0]).name;
        levelUpButtons[2].text = kvps[2].Key == 1 ? "vitality: " + kvps[2].Value[0] + " strength: " + kvps[2].Value[1] + " agility: " + kvps[2].Value[2] + " velocity: " + kvps[2].Value[3] : SkillsArray.GetInstance(kvps[2].Value[0]).name;
    }

    public void SetSomething()
    {
        if (selectedButton == -1) return;

        switch (setType)
        {
            case 0:
                SetWeapon();
                break;
            case 1:
                SetShield();
                break;
            case 2:
                SetPet();
                break;
            default:
                break;
        }
        CloseAddEquipable();
    }

    public void CloseAddEquipable()
    {
        setType = -1;
        selectedButton = -1;
        CustomMainMenuScript.ToggleHeader();
    }

    public async void SetWeapon()
    {
        var xd = new HexBigInteger(0);
        string response = await Moralis.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "SetWeapon", new object[2] { rudoDisplayed.NftId, selectedButton }, xd, xd, xd);
        print(response);
    }
    public async void SetShield()
    {
        var xd = new HexBigInteger(0);
        string response = await Moralis.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "SetShield", new object[2] { rudoDisplayed.NftId, selectedButton }, xd, xd, xd);
        print(response);
    }
    public async void SetPet()
    {
        var xd = new HexBigInteger(0);
        string response = await Moralis.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "SetPet", new object[2] { rudoDisplayed.NftId, selectedButton }, xd, xd, xd);
        print(response);
    }

    public void SelectButton(int selection)
    {
        selectedButton = selection;
    }

    public void InitializeBig(Rudo rudo)
    {
        rudoDisplayed = rudo;
        name.text = rudo.FighterName;
        elo.text = "0";
        nftId.text = rudo.NftId.ToString();
        vitality.text = rudo.Vitality.ToString();
        strength.text = rudo.Strength.ToString();
        agility.text = rudo.Agility.ToString();
        velocity.text = rudo.Velocity.ToString();
        level.text = rudo.Level.ToString();
        expSlider.value = (rudo.Experience-(rudo.Level*2+2))/ ((rudo.Level+1) * 2 + 2);

        //clearWeapons
        foreach (Transform item in weaponGridLayoutGroup.transform)
        {
            Destroy(item.gameObject);
        }
        //drawWeapons
        for(int i = 0; i < rudo.Weapons.Count;i++)
        {
            GameObject go = Instantiate(EquipableButton, weaponGridLayoutGroup.transform);
            go.GetComponent<Image>().sprite = sword;
            go.GetComponent<Button>().onClick.AddListener(delegate { selectedButton = rudo.Weapons[i].Equipable.nftId; });
        }        
        //clearSkills
        foreach (Transform item in skillsGridLayoutGroup.transform)
        {
            Destroy(item.gameObject);
        }
        //drawSkills
        for (int i = 0; i < rudo.skills.Count; i++)
        {
            GameObject go = Instantiate(EquipableButton, skillsGridLayoutGroup.transform);
            go.GetComponentInChildren<TMP_Text>().text = SkillsArray.GetInstance(rudo.skills[i]).name;
        }

        if (rudo.Pet != null)
            petButton.GetComponent<Image>().sprite = pet;
        else
            petButton.GetComponent<Image>().sprite = empty;

        if (rudo.Shield != null)
            shieldButton.GetComponent<Image>().sprite = shield;
        else
            shieldButton.GetComponent<Image>().sprite = empty;
    }

    public async void RemoveWeapon()
    {
        var xd = new HexBigInteger(0);
        string response = await Moralis.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "RemoveWeapon", new object[2] { rudoDisplayed.NftId, selectedButton}, xd, xd, xd);
        print(response);
    }

    public async void RemoveShield()
    {
        var xd = new HexBigInteger(0);
        string response = await Moralis.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "RemoveShield", new object[1] { rudoDisplayed.NftId }, xd, xd, xd);
        print(response);
    }

    public async void RemovePet()
    {
        var xd = new HexBigInteger(0);
        string response = await Moralis.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "RemovePet", new object[1] { rudoDisplayed.NftId }, xd, xd, xd);
        print(response);
    }

    public async void LoadPets(GameObject DisplayLayout)
    {
        setType = 2;
        selectedButton = -1;
        List<GameObject> gol = CustomMainMenuScript.CreateMiniEquipableDisplay(await CustomMainMenuScript.GetEquipables<PetMoralis>());
        foreach (GameObject item in gol)
        {
            item.GetComponent<Button>().onClick.AddListener(delegate { selectedButton = int.Parse(item.GetComponent<LoadMiniEquipable>().nftId.text); });
        }
        CustomMainMenuScript.FillGroupLayout(gol, DisplayLayout);
    }
    public async void LoadShields(GameObject DisplayLayout)
    {
        setType = 1;
        selectedButton = -1;
        List<GameObject> gol = CustomMainMenuScript.CreateMiniEquipableDisplay(await CustomMainMenuScript.GetEquipables<ShieldMoralis>());
        foreach (GameObject item in gol)
        {
            item.GetComponent<Button>().onClick.AddListener(delegate { selectedButton = int.Parse(item.GetComponent<LoadMiniEquipable>().nftId.text); });
        }
        CustomMainMenuScript.FillGroupLayout(gol,DisplayLayout);
    }
    public async void LoadWeapons(GameObject DisplayLayout)
    {
        setType = 0;
        selectedButton = -1;
        List<GameObject> gol = CustomMainMenuScript.CreateMiniEquipableDisplay(await CustomMainMenuScript.GetEquipables<WeaponMoralis>());
        foreach (GameObject item in gol)
        {
            item.GetComponent<Button>().onClick.AddListener(delegate { selectedButton = int.Parse(item.GetComponent<LoadMiniEquipable>().nftId.text); });
        }
        CustomMainMenuScript.FillGroupLayout(gol, DisplayLayout);
    }
    public void BuyExp()
    {
        ContractCalls.BuyExp(rudoDisplayed.NftId, int.Parse(expInput.text));
    }
    public void LevelUp()
    {
        if (selectedButton == -1)
            return;
        ContractCalls.LevelUp(rudoDisplayed.NftId, selectedButton);
    }
}
