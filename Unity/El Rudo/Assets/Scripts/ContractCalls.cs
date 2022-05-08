using MoralisWeb3ApiSdk;
using Nethereum.Hex.HexTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;

public class ContractCalls
{
    public static async void BuyExp(int rudoId, int expAmmount)
    {
        var xd = new HexBigInteger(0);
        string response = await MoralisInterface.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "buyExp", new object[2] { rudoId, expAmmount }, xd, xd, xd);
        Debug.Log(response);
    }
    public static async void LevelUp(int rudoId, int choiseIndex)
    {
        var xd = new HexBigInteger(0);
        string response = await MoralisInterface.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "levelUp", new object[2] { rudoId, choiseIndex }, xd, xd, xd);
        Debug.Log(response);
    }
}
