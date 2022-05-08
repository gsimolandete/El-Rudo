// SPDX-License-Identifier: MIT

pragma solidity ^0.8.12;

import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/token/ERC721/ERC721.sol";
import "@openzeppelin/contracts/token/ERC721/extensions/ERC721Enumerable.sol";
import "@openzeppelin/contracts/utils/Strings.sol";
import "@chainlink/contracts/src/v0.8/interfaces/LinkTokenInterface.sol";
import "@chainlink/contracts/src/v0.8/interfaces/VRFCoordinatorV2Interface.sol";
import "@chainlink/contracts/src/v0.8/VRFConsumerBaseV2.sol";
import "./SetupRandom.sol";
import "./RudoAccessControl.sol";

abstract contract RudoEquipable is ERC721Enumerable, SetupRandom, RudoAccessControl {
  constructor(address addr, string memory _name, string memory _symbol)
    SetupRandom(198)
    ERC721(_name, _symbol)
    RudoAccessControl(addr)
    {

    }

    struct RudoEquipableStruct {
        uint16 id;
        uint16 quality;
        bool equiped;
    }

    event NewEquipable(address owner, uint id, uint16 weaponId, uint16 weaponQuality);

    RudoEquipableStruct[] public equipables;

    uint16[] MAX = [1000,2000,3000,4000];
    uint16[] Chances = [100,1500,5000,10000];
    uint16[] Existings = [5,10,15,20];

    uint256 COUNTER=0;

    uint32 callbackGasLimit = 1800000; //careXD
    uint16 requestConfirmations = 3;

    function IsEquiped(uint256 equipableId) public view returns (bool){
        return equipables[equipableId].equiped;
    }

    function Unequip(uint256 equipableId) public onlyRudoContract{
        equipables[equipableId].equiped = false;
    }

    function Equip(uint256 equipableId) public onlyRudoContract{
        equipables[equipableId].equiped = true;
    }

    function fulfillRandomWords(
        uint256 requestId,
        uint256[] memory randomWords
    ) internal override {
        _createEquipable(requestId, randomWords[0]);
    }

    function RequestRandomEquipable() public payable {
        require(msg.value >= variables.fee());
        uint256 s_requestId = COORDINATOR.requestRandomWords(
        keyHash,
        s_subscriptionId,
        requestConfirmations,
        callbackGasLimit,
        1
        );
        requestToSender[s_requestId] = msg.sender;
    }  
    
    function _createEquipable(uint256 requestId, uint256 _randomWord) internal {
        equipables.push();
        equipables[COUNTER].id = randomEquipableId(_randomWord);
        equipables[COUNTER].quality = uint16(_randomWord/100 % 50 + 1);
        _safeMint(requestToSender[requestId], COUNTER);
        emit NewEquipable(requestToSender[requestId], COUNTER, equipables[COUNTER].id, equipables[COUNTER].quality);
        COUNTER++;
    }

    function _createTestEquipable() public payable {
        equipables.push();
        equipables[COUNTER].id = randomEquipableId(block.timestamp);
        equipables[COUNTER].quality = uint16(block.timestamp/100 % 50 + 50);
        _safeMint(msg.sender, COUNTER);
        emit NewEquipable(msg.sender, COUNTER, equipables[COUNTER].id, equipables[COUNTER].quality);
        COUNTER++;
    }

    function randomEquipableId(uint256 _randomWord) internal view returns (uint16){
        uint16 equipableId;
        uint256 _randomChance = _randomWord%Chances[3];
        uint16 offset = 0;
        for(uint8 i = 0; i < 4; i++){
            if(_randomChance < Chances[i]){
                equipableId = uint16(_randomWord % Existings[i]) + offset;
                return equipableId;
            }
            offset += MAX[i];
        }
        return equipableId;
    }
}