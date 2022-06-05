using MoralisUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using MoralisUnity.Platform.Objects;
using MoralisUnity.Platform.Queries;
using Cysharp.Threading.Tasks;
using MoralisUnity.Web3Api.Models;
using MoralisUnity.Web3Api.Client;
using System;
using MoralisUnity.Web3Api.Core.Models;
using MoralisUnity.Web3Api.CloudApi;
using Newtonsoft.Json;
using MoralisUnity.Web3Api.Core;
using System.Net;

public class ContractCalls
{
    public static async void BuyExp(int rudoId, int expAmmount)
    {
        var xd = new HexBigInteger(0);
        string response = await Moralis.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "buyExp", new object[2] { rudoId, expAmmount }, xd, xd, xd);
        Debug.Log(response);
    }
    public static async void LevelUp(int rudoId, int choiseIndex)
    {
        var xd = new HexBigInteger(0);
        string response = await Moralis.ExecuteContractFunction(RUDO_CONTRACT_ADDRESS, RUDO_ABI, "levelUp", new object[2] { rudoId, choiseIndex }, xd, xd, xd);
        Debug.Log(response);
    }
	public static async UniTask<T> RunContractFunction<T>(string address, string functionName, RunContractDto abi, ChainList chain, string subdomain = null, string providerUrl = null)
	{

		// Verify the required parameter 'address' is set
		if (address == null) throw new ApiException(400, "Missing required parameter 'address' when calling RunContractFunction");

		// Verify the required parameter 'functionName' is set
		if (functionName == null) throw new ApiException(400, "Missing required parameter 'functionName' when calling RunContractFunction");

		// Verify the required parameter 'abi' is set
		if (abi == null) throw new ApiException(400, "Missing required parameter 'abi' when calling RunContractFunction");

		var postBody = new Dictionary<String, object>();
		var queryParams = new Dictionary<String, String>();
		var headerParams = new Dictionary<String, String>();
		var formParams = new Dictionary<String, String>();
		var fileParams = new Dictionary<String, FileParameter>();
		ApiClient ApiClient = (Moralis.GetClient().Web3Api.Native as NativeApi).ApiClient;

		var path = "/functions/runContractFunction";
		if (address != null) postBody.Add("address", ApiClient.ParameterToString(address));
		if (functionName != null) postBody.Add("function_name", ApiClient.ParameterToString(functionName));
		if (abi != null) postBody.Add("abi", abi.Abi);
		if (abi != null) postBody.Add("params", abi.Params);
		if (subdomain != null) postBody.Add("subdomain", ApiClient.ParameterToString(subdomain));
		if (providerUrl != null) postBody.Add("providerUrl", ApiClient.ParameterToString(providerUrl));
		postBody.Add("chain", ApiClient.ParameterToHex((long)chain));

		// Authentication setting, if any
		String[] authSettings = new String[] { "ApiKeyAuth" };

		string bodyData = postBody.Count > 0 ? JsonConvert.SerializeObject(postBody) : null;

		Tuple<HttpStatusCode, Dictionary<string, string>, string> response =
			await ApiClient.CallApi(path, Method.POST, queryParams, bodyData, headerParams, formParams, fileParams, authSettings);

		if (((int)response.Item1) >= 400)
			throw new ApiException((int)response.Item1, "Error calling RunContractFunction: " + response.Item3, response.Item3);
		else if (((int)response.Item1) == 0)
			throw new ApiException((int)response.Item1, "Error calling RunContractFunction: " + response.Item3, response.Item3);

		return ((CloudFunctionResult<T>)ApiClient.Deserialize(response.Item3, typeof(CloudFunctionResult<T>), response.Item2)).Result;
	}
}
