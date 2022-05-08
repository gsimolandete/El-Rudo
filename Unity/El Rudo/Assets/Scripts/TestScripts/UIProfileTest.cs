using Moralis.Platform.Objects;
using Moralis.Web3Api.Models;
using MoralisWeb3ApiSdk;
using Nethereum.Hex.HexTypes;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using UnityEngine;

public class UIProfileTest : MonoBehaviour
{
    public LoadMiniRudoDisplay[] LoadMiniRudoDisplay;
    // Start is called before the first frame update
    public async void LoadRudos()
    {
        //MoralisUser user = await MoralisInterface.GetUserAsync();
        //string addr = user.authData["moralisEth"]["id"].ToString();

        //NftOwnerCollection noc =
        //await MoralisInterface.GetClient().Web3Api.Account.GetNFTsForContract(addr.ToLower(),
        //Constants.MUG_CONTRACT_ADDRESS,
        //ChainList.bsc_testnet);
        //IEnumerable<NftOwner> ownership = from n in noc.Result
        //                                    select n;

        //print(ownership);

        //var response = await CallContract("getRudosFromAddress2", "[\""+addressInput.text+"\"]");
        //AbstractRudo[] abstractRudos = SerializeObjects.StringToAbstractRudo(response);
        //for (int i = 0; i < 10; i++)
        //{
        //    if (i < abstractRudos.Length)
        //    {
        //        LoadMiniRudoDisplay[i].gameObject.SetActive(true);
        //        LoadMiniRudoDisplay[i].Initialize(abstractRudos[i]);
        //    }
        //    else
        //        LoadMiniRudoDisplay[i].gameObject.SetActive(false);
        //}
    }

    public async void MintRudo()
    {

        // Need the user for the wallet address
        MoralisUser user = await MoralisInterface.GetUserAsync();

        string addr = user.authData["moralisEth"]["id"].ToString();

        // Convert token id to integer
        BigInteger bi = 0;

#if UNITY_WEBGL

        // Convert token id to hex as this is what the contract call expects
        object[] pars = new object[] { bi.ToString() };

        // Set gas estimate
        HexBigInteger gas = new HexBigInteger(0);
        string resp = await MoralisInterface.ExecuteFunction(Constants.MUG_CONTRACT_ADDRESS, Constants.MUG_ABI, Constants.MUG_CLAIM_FUNCTION, pars, new HexBigInteger("0x0"), gas, gas);
#else

        // Convert token id to hex as this is what the contract call expects
        //object[] pars = new object[] { mintNameInput.text };

        // Set gas estimate
        HexBigInteger gas = new HexBigInteger(0);
        // Call the contract to claim the NFT reward.
        //string resp = await MoralisInterface.SendEvmTransactionAsync("Rudo", "binance", Constants.MUG_MINT_RUDO, addr, gas, new HexBigInteger("0x0"), pars);
#endif
        // Hide the NFT GameObject since it has been claimed
        // You could also play a victory sound etc.
        transform.gameObject.SetActive(false);
    }
}
