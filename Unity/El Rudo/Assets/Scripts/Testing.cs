using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MoralisUnity;
using static GlobalVariables;
using System.Threading.Tasks;
using MoralisUnity.Platform.Queries;
using MoralisUnity.Web3Api.Models;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;
using System;
using Nethereum.Web3;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Numerics;

public class Testing : MonoBehaviour
{
    private void Start()
    {
    }

    [Function("rudos", "string")]
    public class RudosFunction : FunctionMessage
    {
        [Parameter("uint256", "id", 1)]
        public int Id { get; set; }
    }

    [Function("equipables", "string")]
    public class EquipablesFunction : FunctionMessage
    {
        [Parameter("uint256", "id", 1)]
        public int Id { get; set; }
    }

    [FunctionOutput]
    public class EquipableBC : IFunctionOutputDTO
    {
        [Parameter("uint16", "id", 1)]
        public int Id { get; set; }

        [Parameter("uint16", "quality", 2)]
        public int Quality { get; set; }

        [Parameter("bool", "equiped", 3)]
        public bool Equiped { get; set; }
    }

    [FunctionOutput]
    public class RudoBC : IFunctionOutputDTO
    {
        [Parameter("string", "name", 1)]
        public string Name { get; set; }

        [Parameter("uint16", "level", 2)]
        public int Level { get; set; }

        [Parameter("uint32", "experience", 3)]
        public int Experience { get; set; }

        [Parameter("uint16", "nature", 4)]
        public int Nature { get; set; }

        [Parameter("tuple", "stats", 5)]
        public Stats Stats { get; set; }

        [Parameter("bool", "nextSkillsReady", 6)]
        public bool NextSkillsReady { get; set; }

        [Parameter("int32", "elo", 7)]
        public int Elo { get; set; }

        [Parameter("tuple", "pet", 8)]
        public SlotBC Pet { get; set; }

        [Parameter("tuple", "shield", 9)]
        public SlotBC Shield { get; set; }

        [Parameter("tuple", "energy", 10)]
        public RudoEnergy Energy { get; set; }
    }
    [FunctionOutput]
    public class Stats : IFunctionOutputDTO
    {
        [Parameter("uint16", "vitality", 1)]
        public int Vitality { get; set; }

        [Parameter("uint16", "strength", 2)]
        public int Strength { get; set; }

        [Parameter("uint16", "agility", 3)]
        public int Agility { get; set; }

        [Parameter("uint16", "velocity", 4)]
        public int Velocity { get; set; }
    }
    [FunctionOutput]
    public class SlotBC : IFunctionOutputDTO
    {
        [Parameter("bool", "slotUsed", 1)]
        public bool SlotUsed { get; set; }

        [Parameter("uint256", "Id", 2)]
        public int Id { get; set; }
    }

    [FunctionOutput]
    public class RudoEnergy : IFunctionOutputDTO
    {
        [Parameter("uint32", "fullEnergyTime", 1)]
        public int FullEnergyTime { get; set; }
    }
    public async void test1()
    {
        int rudoid = 15;

        var web3 = new Web3("https://speedy-nodes-nyc.moralis.io/963a7e8bb028bea0aeb3722d/polygon/mumbai");
        var imputRudos = new RudosFunction() {  Id = rudoid };

        var rudobcHandler = web3.Eth.GetContractQueryHandler<RudosFunction>();
        var equipablesHandler = web3.Eth.GetContractQueryHandler<EquipablesFunction>();
        var rudobc = await rudobcHandler.QueryDeserializingToObjectAsync<RudoBC>(imputRudos, RUDO_CONTRACT_ADDRESS);

        EquipableBC petbc = null;
        EquipableBC shieldbc = null;

        if (rudobc.Pet.SlotUsed)
        {
            var imputPets = new EquipablesFunction() { Id = rudobc.Pet.Id };
            petbc = await equipablesHandler.QueryDeserializingToObjectAsync<EquipableBC>(imputPets, PET_CONTRACT_ADDRESS);
        }
        if (rudobc.Shield.SlotUsed)
        {
            var imputShields = new EquipablesFunction() { Id = rudobc.Shield.Id };
            shieldbc = await equipablesHandler.QueryDeserializingToObjectAsync<EquipableBC>(imputShields, SHIELD_CONTRACT_ADDRESS);
        }


        Rudo rudo = new Rudo(rudoid,rudobc.Experience, rudobc.Name, rudobc.Level, rudobc.Stats.Vitality, rudobc.Stats.Strength, rudobc.Stats.Velocity, rudobc.Stats.Agility, null, rudobc.Pet.SlotUsed ? new Pet(0,petbc.Id,0) : null, rudobc.Shield.SlotUsed ? new Shield(0,shieldbc.Id,0) : null, null);
        print(rudo.FighterName);
    }
    public void test2()
    {
    }
}
