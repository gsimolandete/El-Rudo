using MoralisWeb3ApiSdk;
using Nethereum.Hex.HexTypes;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GlobalVariables;

public class RudoViewerScript : LoadMiniRudoDisplay
{
    [SerializeField]
    GameObject weaponGridLayoutGroup, petButton, shieldButton, EquipableButton;
    [SerializeField]
    Sprite sword, shield, pet, empty;
    [SerializeField]
    TMP_InputField expInput;
    [SerializeField]
    CustomMainMenuScript CustomMainMenuScript;
    [SerializeField]
    Slider expSlider;

    int setType;
    int selectedButton;

    Rudo rudoDisplayed;

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
        selectedButton = -1;
        CustomMainMenuScript.ToggleHeader();
    }

    public async void SetWeapon()
    {
        var xd = new HexBigInteger(0);
        string response = await MoralisInterface.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "SetWeapon", new object[2] { rudoDisplayed.NftId, selectedButton }, xd, xd, xd);
        print(response);
    }
    public async void SetShield()
    {
        var xd = new HexBigInteger(0);
        string response = await MoralisInterface.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "SetShield", new object[2] { rudoDisplayed.NftId, selectedButton }, xd, xd, xd);
        print(response);
    }
    public async void SetPet()
    {
        var xd = new HexBigInteger(0);
        string response = await MoralisInterface.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "SetPet", new object[2] { rudoDisplayed.NftId, selectedButton }, xd, xd, xd);
        print(response);
    }

    public void InitializeBig(Rudo rudo)
    {
        rudoDisplayed = rudo;
        name.text = rudo.FighterName;
        elo.text = "0";
        vitality.text = rudo.Vitality.ToString();
        strength.text = rudo.Strength.ToString();
        agility.text = rudo.Agility.ToString();
        velocity.text = rudo.Velocity.ToString();
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
            go.GetComponent<Button>().onClick.AddListener(delegate { selectedButton = i; });
        }

        if(rudo.Pet != null)
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
        string response = await MoralisInterface.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "RemoveWeapon", new object[2] { rudoDisplayed.NftId, selectedButton}, xd, xd, xd);
        print(response);
    }

    public async void RemoveShield()
    {
        var xd = new HexBigInteger(0);
        string response = await MoralisInterface.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "RemoveShield", new object[1] { rudoDisplayed.NftId }, xd, xd, xd);
        print(response);
    }

    public async void RemovePet()
    {
        var xd = new HexBigInteger(0);
        string response = await MoralisInterface.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "RemovePet", new object[1] { rudoDisplayed.NftId }, xd, xd, xd);
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
}
