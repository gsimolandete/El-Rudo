using Moralis.Platform.Objects;
using Moralis.Web3Api.Models;
using MoralisWeb3ApiSdk;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WalletConnectSharp.Unity;
using static GlobalVariables;
using Nethereum.Hex.HexTypes;
using System;
using Moralis.Platform.Queries;
using UnityEngine.UI;
using System.Threading.Tasks;

public class CustomMainMenuScript : MonoBehaviour
{
    public MainMenuScript menuScript;
    public WalletConnect walletConnect;
    public GameObject qrMenu;
    public GameObject miniRudoDisplayTemplate;
    public GameObject miniEquipableDisplayTemplate;
    public List<GameObject> menus;
    public GameObject connectWalletMenu;
    public GameObject header;
    public static List<Rudo> ownRudos;


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

        MoralisQuery<T> query = MoralisInterface.GetClient().Query<T>().WhereEqualTo("owner", MoralisInterface.GetUser().ethAddress);
        List<T> result = await query.FindAsync() as List<T>;

        return result;
    }

    public async Task<List<Rudo>> GetRudos()
    {
        MoralisQuery<RudoMoralis> query = MoralisInterface.GetClient().Query<RudoMoralis>().WhereEqualTo("owner", MoralisInterface.GetUser().ethAddress);
        List<RudoMoralis> result = await query.FindAsync() as List<RudoMoralis>;
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
        MoralisQuery<RudoMoralis> query = MoralisInterface.GetClient().Query<RudoMoralis>().WhereEqualTo("rudoId", rudoId);
        List<RudoMoralis> result = await query.FindAsync() as List<RudoMoralis>;

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
            MoralisQuery<WeaponMoralis> queryWeapons = MoralisInterface.GetClient().Query<WeaponMoralis>().WhereContainedIn("objectId", aaaa);
            List<WeaponMoralis> resultWeapon = await queryWeapons.FindAsync() as List<WeaponMoralis>;

            for (int u2 = 0; u2 < resultWeapon.Count; u2++)
            {
                string pathToAddressable = "";
                weapons.Add(new Weapon(resultWeapon[u2].nftId, resultWeapon[u2].weaponId, resultWeapon[u2].weaponQuality, pathToAddressable, WeaponsArray.GetInstance(resultWeapon[u2].weaponId)));
            }
        }

        Pet pet = null;
        if (result.pet != null)
        {
            MoralisQuery<PetMoralis> queryPet = MoralisInterface.GetClient().Query<PetMoralis>().WhereEqualTo("objectId", result.pet.objectId);
            List<PetMoralis> resultPet = await queryPet.FindAsync() as List<PetMoralis>;
            string pathToAddressable = "";
            pet = resultPet.Count > 0 ? new Pet(resultPet[0].nftId, resultPet[0].weaponId, resultPet[0].weaponQuality, pathToAddressable, "pet", 5, 5, 5, 5) : null;
        }

        Shield shield = null;
        if (result.shield != null)
        {
            MoralisQuery<ShieldMoralis> queryShield = MoralisInterface.GetClient().Query<ShieldMoralis>().WhereEqualTo("objectId", result.shield.objectId);
            List<ShieldMoralis> resultShield = await queryShield.FindAsync() as List<ShieldMoralis>;
            string pathToAddressable = "";
            shield = resultShield.Count > 0 ? new Shield(resultShield[0].nftId, resultShield[0].weaponId, resultShield[0].weaponQuality, pathToAddressable) : null;
        }

        return new Rudo(result.rudoId, result.experience, result.name, result.level, result.vitality, result.strength, result.velocity, result.agility, weapons, pet, shield);
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
            go.GetComponent<LoadMiniRudoDisplay>().InitializeMini(result[i],this,menus[5]);
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
        string response = await MoralisInterface.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "_createRudo2", new object[0], xd, xd, xd);
        print(response);
    }
    public static async void MintShield()
    {
        var xd = new HexBigInteger(0);
        string response = await MoralisInterface.ExecuteContractFunction(SHIELD_CONTRACT_ADDRESS, EQUIPABLE_ABI, "_createTestEquipable", new object[0], xd, xd, xd);
        print(response);
    }
    public static async void MintPet()
    {
        var xd = new HexBigInteger(0);
        string response = await MoralisInterface.ExecuteContractFunction(PET_CONTRACT_ADDRESS, EQUIPABLE_ABI, "_createTestEquipable", new object[0], xd, xd, xd);
        print(response);
    }
    public static async void MintWeapon()
    {
        var xd = new HexBigInteger(0);
        string response = await MoralisInterface.ExecuteContractFunction(WEAPON_CONTRACT_ADDRESS, EQUIPABLE_ABI, "_createTestEquipable", new object[0], xd, xd, xd);
        print(response);
    }
    public static async void SetWeapon(int rudoId, int weaponId)
    {
        var xd = new HexBigInteger(0);
        string response = await MoralisInterface.ExecuteContractFunction(WEAPON_CONTRACT_ADDRESS, EQUIPABLE_ABI, "SetWeapon", new object[2] { rudoId, weaponId }, xd, xd, xd);
        print(response);
    }
    public static async void SetShield(int rudoId, int weaponId)
    {
        var xd = new HexBigInteger(0);
        string response = await MoralisInterface.ExecuteContractFunction(WEAPON_CONTRACT_ADDRESS, EQUIPABLE_ABI, "SetShield", new object[2] { rudoId, weaponId }, xd, xd, xd);
        print(response);
    }
    public static async void SetPet(int rudoId, int weaponId)
    {
        var xd = new HexBigInteger(0);
        string response = await MoralisInterface.ExecuteContractFunction(WEAPON_CONTRACT_ADDRESS, EQUIPABLE_ABI, "SetPet", new object[2] { rudoId, weaponId }, xd, xd, xd);
        print(response);
    }

    public void ToggleHeader()
    {
        header.SetActive(!header.activeSelf);
    }

    public async void Authoentificate()
    {
        // If the user is still logged in just show game.
        if (MoralisInterface.IsLoggedIn())
        {
            Debug.Log("User is already logged in to Moralis.");
            connectWalletMenu.SetActive(false);
        }
        else
        {
            Debug.Log("User is not logged in.");
#if UNITY_ANDROID || UNITY_IOS
        walletConnect.OpenDeepLink();
#elif UNITY_WEBGL
        await menuScript.LoginWithWeb3();
#else
            qrMenu.SetActive(true);
            qrMenu.GetComponentInChildren<WalletConnectQRImage>().WalletConnectOnConnectionStarted();
#endif
        }
    }
}
