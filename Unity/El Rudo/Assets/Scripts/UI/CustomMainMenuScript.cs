using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WalletConnectSharp.Unity;
using static GlobalVariables;
using System;
using UnityEngine.UI;
using System.Threading.Tasks;
using MoralisUnity;
using MoralisUnity.Platform.Queries;
using Nethereum.Hex.HexTypes;
using MoralisUnity.Kits.AuthenticationKit;
using TMPro;
using MoralisUnity.Web3Api.Models;

public class CustomMainMenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject authenticationKitObject = null;
    private AuthenticationKit authKit = null;

    public WalletConnect walletConnect;
    public GameObject miniRudoDisplayTemplate;
    public GameObject miniEquipableDisplayTemplate;
    public List<GameObject> menus;
    public GameObject header;
    public GameObject connectMenu;
    public static List<Rudo> ownRudos;
    public TMP_Text maticBalance;

    private void Start()
    {
        authKit = authenticationKitObject.GetComponent<AuthenticationKit>();
    }

    public void NavigateTo(GameObject menu)
    {
        foreach(GameObject go in menus)
        {
            go.SetActive(false);
        }
        menu.SetActive(true);
    }

    public async void LoadRudos(GameObject DisplayLayout)
    {
        FillGroupLayout(CreateMiniRudoDisplay(await GetRudos()), DisplayLayout);
    }

    public async void LoadPets(GameObject DisplayLayout)
    {
        FillGroupLayout(CreateMiniEquipableDisplay(await GetEquipables<PetMoralis>()), DisplayLayout);
    }
    public async void LoadShields(GameObject DisplayLayout)
    {
        FillGroupLayout(CreateMiniEquipableDisplay(await GetEquipables<ShieldMoralis>()), DisplayLayout);
    }
    public async void LoadWeapons(GameObject DisplayLayout)
    {
        FillGroupLayout(CreateMiniEquipableDisplay(await GetEquipables<WeaponMoralis>()), DisplayLayout);
    }

    public async Task<List<T>> GetEquipables<T>() where T : EquipableMoralis
    {
        MoralisQuery<T> q = await Moralis.Query<T>();
        q = q.WhereEqualTo("owner", (await Moralis.GetUserAsync()).ethAddress);
        return  await q.FindAsync() as List<T>;
    }

    public async Task<List<Rudo>> GetRudos()
    {
        MoralisQuery<RudoMoralis> q = await Moralis.Query<RudoMoralis>();
        q = q.WhereEqualTo("owner", (await Moralis.GetUserAsync()).ethAddress);
        List<RudoMoralis> result = await q.FindAsync() as List<RudoMoralis>;

        List<Rudo> rudoList = new List<Rudo>();

        for (int i = 0; i < result.Count; i++)
        {
            rudoList.Add(await ProcessRudo(result[i]));
        }

        ownRudos = rudoList;

        return rudoList;
    }
    public async Task<Rudo> GetRudo(int rudoId)
    {
        MoralisQuery<RudoMoralis> q = await Moralis.Query<RudoMoralis>();
        q = q.WhereEqualTo("rudoId", rudoId);
        List<RudoMoralis> result = await q.FindAsync() as List<RudoMoralis>;

        return await ProcessRudo(result[0]);
    }

    public async Task<Rudo> ProcessRudo(RudoMoralis result)
    {
        List<Weapon> weapons = new List<Weapon>();

        if (result.weapons != null)
        {
            string[] aaaa = new string[result.weapons.Count];
            for (int u = 0; u < result.weapons.Count; u++)
            {
                aaaa[u] = result.weapons[u].objectId.ToString();
            }
            List<WeaponMoralis> resultWeapon = new List<WeaponMoralis>();
            for (int u = 0; u < result.weapons.Count; u++)
            {
                MoralisQuery<WeaponMoralis> q = await Moralis.Query<WeaponMoralis>();
                q = q.WhereEqualTo("objectId", aaaa[u]);
                resultWeapon.Add((await q.FindAsync() as List<WeaponMoralis>)[0]);

            }


            for (int u2 = 0; u2 < resultWeapon.Count; u2++)
            {
                weapons.Add(new Weapon(resultWeapon[u2].nftId, resultWeapon[u2].weaponId, resultWeapon[u2].weaponQuality));
            }
        }

        Pet pet = null;
        if (result.pet != null)
        {
            MoralisQuery<PetMoralis> q = await Moralis.Query<PetMoralis>();
            q = q.WhereEqualTo("objectId", result.pet.objectId);
            List<PetMoralis> resultPet = await q.FindAsync() as List<PetMoralis>;

            pet = resultPet.Count > 0 ? new Pet(resultPet[0].nftId, resultPet[0].weaponId, resultPet[0].weaponQuality) : null;
        }

        Shield shield = null;
        if (result.shield != null)
        {
            MoralisQuery<ShieldMoralis> q = await Moralis.Query<ShieldMoralis>();
            q = q.WhereEqualTo("objectId", result.shield.objectId);
            List<ShieldMoralis> resultShield = await q.FindAsync() as List<ShieldMoralis>;

            shield = resultShield.Count > 0 ? new Shield(resultShield[0].nftId, resultShield[0].weaponId, resultShield[0].weaponQuality) : null;
        }

        return new Rudo(result.rudoId, result.experience, result.name, result.level, result.vitality, result.strength, result.velocity, result.agility, weapons, pet, shield, result.skills == null ? new List<int>() : result.skills);
    }
    public List<GameObject> CreateMiniEquipableDisplay<T>(List<T> result) where T : EquipableMoralis
    {
        List<GameObject> gol = new List<GameObject>();
        for (int i = 0; i < result.Count; i++)
        {
            GameObject go;
            go = Instantiate(miniEquipableDisplayTemplate);
            go.GetComponent<LoadMiniEquipable>().Initialize(result[i]);
            gol.Add(go);
        }
        return gol;
    }

    public List<GameObject> CreateMiniRudoDisplay(List<Rudo> result)
    {
        List<GameObject> gol = new List<GameObject>();
        for (int i = 0; i < result.Count; i++)
        {
            GameObject go;
            go = Instantiate(miniRudoDisplayTemplate);
            go.GetComponent<LoadMiniRudoDisplay>().InitializeMini(result[i],this,menus[4]);
            gol.Add(go);
        }
        return gol;
    }

    public void FillGroupLayout(List<GameObject> go, GameObject DisplayScroller)
    {
        DisplayScroller.GetComponent<GridLayoutGroup>().cellSize = new Vector2(go[0].GetComponent<RectTransform>().rect.width, go[0].GetComponent<RectTransform>().rect.height);

        //clear layout group
        foreach (Transform child in DisplayScroller.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < go.Count; i++)
        {
            go[i].transform.SetParent(DisplayScroller.transform,false);
        }
    }

    public static async void MintRudo()
    {
        var xd = new HexBigInteger(0);
        string response = await Moralis.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "_createRudo2", new object[0], xd, xd, xd);
        print(response);
    }
    public static async void MintShield()
    {
        var xd = new HexBigInteger(0);
        string response = await Moralis.ExecuteContractFunction(SHIELD_CONTRACT_ADDRESS, EQUIPABLE_ABI, "_createTestEquipable", new object[0], xd, xd, xd);
        print(response);
    }
    public static async void MintPet()
    {
        var xd = new HexBigInteger(0);
        string response = await Moralis.ExecuteContractFunction(PET_CONTRACT_ADDRESS, EQUIPABLE_ABI, "_createTestEquipable", new object[0], xd, xd, xd);
        print(response);
    }
    public static async void MintWeapon()
    {
        var xd = new HexBigInteger(0);
        string response = await Moralis.ExecuteContractFunction(WEAPON_CONTRACT_ADDRESS, EQUIPABLE_ABI, "_createTestEquipable", new object[0], xd, xd, xd);
        print(response);
    }
    public static async void SetWeapon(int rudoId, int weaponId)
    {
        var xd = new HexBigInteger(0);
        string response = await Moralis.ExecuteContractFunction(WEAPON_CONTRACT_ADDRESS, EQUIPABLE_ABI, "SetWeapon", new object[2] { rudoId, weaponId }, xd, xd, xd);
        print(response);
    }
    public static async void SetShield(int rudoId, int weaponId)
    {
        var xd = new HexBigInteger(0);
        string response = await Moralis.ExecuteContractFunction(WEAPON_CONTRACT_ADDRESS, EQUIPABLE_ABI, "SetShield", new object[2] { rudoId, weaponId }, xd, xd, xd);
        print(response);
    }
    public static async void SetPet(int rudoId, int weaponId)
    {
        var xd = new HexBigInteger(0);
        string response = await Moralis.ExecuteContractFunction(WEAPON_CONTRACT_ADDRESS, EQUIPABLE_ABI, "SetPet", new object[2] { rudoId, weaponId }, xd, xd, xd);
        print(response);
    }

    public void ToggleHeader()
    {
        header.SetActive(!header.activeSelf);
    }
    public async void Authentication_OnConnect()
    {
        authenticationKitObject.SetActive(false);
        connectMenu.SetActive(false);
        LoadBalance();
    }
    public async void LoadBalance()
    {
        double balance = 0.0;
        NativeBalance balanceResponse = (await Moralis.Web3Api.Account.GetNativeBalance((await Moralis.GetUserAsync()).ethAddress, Moralis.CurrentChain.EnumValue));
        string sym = Moralis.CurrentChain.Symbol;
        float decimals = Moralis.CurrentChain.Decimals * 1.0f;
        double.TryParse(balanceResponse.Balance, out balance);
        maticBalance.text = string.Format("{0:0.####} {1}", (balance / (double)Mathf.Pow(10.0f, decimals)), sym);
    }

    public void LogoutButton_OnClicked()
    {
        // Logout the Moralis User.
        authKit.Disconnect();

        authenticationKitObject.SetActive(true);
    }
}
