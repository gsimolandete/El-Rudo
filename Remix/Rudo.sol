// SPDX-License-Identifier: MIT

pragma solidity ^0.8.12;

import "@openzeppelin/contracts/token/ERC721/ERC721.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/token/ERC721/extensions/ERC721Enumerable.sol";
import "@openzeppelin/contracts/utils/Strings.sol";
import "@chainlink/contracts/src/v0.8/interfaces/LinkTokenInterface.sol";
import "@chainlink/contracts/src/v0.8/interfaces/VRFCoordinatorV2Interface.sol";
import "@chainlink/contracts/src/v0.8/VRFConsumerBaseV2.sol";
import "./RudoAccessControl.sol";

uint8 constant MAXRUDOS = 10;
uint8 constant MAXEPICNATURES = 100;
uint8 constant MAXRARENATURES = 100;
uint8 constant MAXCOMMONNATURES = 100;
uint8 constant TOCHOSESKILLSCOUNT = 3;

contract Rudo is ERC721Enumerable, SetupRandom, RudoAccessControl {
  constructor(string memory _name, string memory _symbol)
    SetupRandom(vrfCoordinator)
    ERC721(_name, _symbol)
  {
    COORDINATOR = VRFCoordinatorV2Interface(vrfCoordinator);
    LINKTOKEN = LinkTokenInterface(link);
    s_subscriptionId = 198;

  }

  uint32 callbackGasLimit = 1800000; //careXD
  uint16 requestConfirmations = 3;
  mapping(uint256 => string) public requestToCharacterName;//try to reduce mappings by making them the same value type?
  mapping(uint256 => address) public requestToSender;
  //-------
  uint256 COUNTER;
  uint256 fee = 0 ether;
  //LEVELS
  uint32[] experienceRequiered = [4,6,8,10,12,14,16,18,20,22,24,26,28,30,32,34,36,38,40];
  //NATURES
  uint16[4] naturesChance = [100,1500,5000,10000];
  uint8[4] naturesCount = [5,10,15,20];
  //RUDOS
  RudoStruct[] public rudos;

  struct RudoStruct {
    string name;
    uint8 level;
    uint8 nature;
    uint16 vitality;
    uint16 strength;
    uint16 agility;
    uint16 velocity;
    uint16 experience;//uint32?
    uint16[] skills;
    bool nextSkillsReady; //implement
    LevelIncreaseChoise[TOCHOSESKILLSCOUNT] nextSkills;
  }

  function GetSkills(uint256 id) public view returns (uint16[] memory){
    return rudos[id].skills;
  }

  function GetLevelIncreases(uint256 id) public view returns (LevelIncreaseChoise[TOCHOSESKILLSCOUNT] memory){
    return rudos[id].nextSkills;
  }

  struct LevelIncreaseChoise {
    LvlIncType choiseType; //1 == skill, 0 == stats
    uint16[4] statsIncrease;
  }

  enum LvlIncType{ SKILL, STATS }
  enum RandomMethod{ CREATERUDO, NEXTSKILLS }

  event NewRudo (address owner, uint256 id, string name, uint16 vitality, uint16 strength, uint16 agility, uint16 velocity);
  event NewDuel (uint256 rudoChallenging, uint256 rudoChallenged, uint256 seed, uint256 version);
  event GainExp (uint256 id, uint32 experience);
  event LevelUp_Skill (uint256 id, uint8 level, uint16 skillId);
  event LevelUp_Stats (uint256 id, uint8 level, uint16 vitality, uint16 strength, uint16 agility, uint16 velocity);
  event Log (string logMessage, uint256 logNumber);

  function updateFee(uint256 _fee) external onlyOwner {
    fee = _fee;
  }

  function updateCallbackGasLimit(uint32 _callbackGasLimit) external onlyOwner{
    callbackGasLimit = _callbackGasLimit;
  }

  function fulfillRandomWords(
    uint256 requestId,
    uint256[] memory randomWords
  ) internal override {
    _createRudo(requestId, randomWords[0]);
  }

  function RequestRandomRudo(string memory _name) public payable {
    require(msg.value >= fee);
    uint256 s_requestId = COORDINATOR.requestRandomWords(
      keyHash,
      s_subscriptionId,
      requestConfirmations,
      callbackGasLimit,
      1
    );
    requestToCharacterName[s_requestId] = _name;
    requestToSender[s_requestId] = msg.sender;
  }

  function buyExp(uint256 rudoId, uint16 ammount) public payable {
    rudos[rudoId].experience += ammount;
    emit GainExp(rudoId,rudos[rudoId].experience);
  }

  function levelUp(uint256 rudoId, uint8 choiseIndex) public payable{
    require(rudos[rudoId].experience >= experienceRequiered[rudos[rudoId].level-1], "Not enought experience");
    require(choiseIndex >= 0 && choiseIndex < 3, "Out of bounds choise");
    require(rudos[rudoId].nextSkillsReady == true);
    if(rudos[rudoId].nextSkills[choiseIndex].choiseType==LvlIncType.STATS){
      rudos[rudoId].vitality += rudos[rudoId].nextSkills[choiseIndex].statsIncrease[0];
      rudos[rudoId].strength += rudos[rudoId].nextSkills[choiseIndex].statsIncrease[1];
      rudos[rudoId].agility += rudos[rudoId].nextSkills[choiseIndex].statsIncrease[2];
      rudos[rudoId].velocity += rudos[rudoId].nextSkills[choiseIndex].statsIncrease[3];
      rudos[rudoId].nextSkillsReady = false;
      emit LevelUp_Stats(rudoId,rudos[rudoId].level,rudos[rudoId].vitality,rudos[rudoId].strength,rudos[rudoId].agility,rudos[rudoId].velocity);
    } else if (rudos[rudoId].nextSkills[choiseIndex].choiseType==LvlIncType.SKILL){
      rudos[rudoId].skills.push(rudos[rudoId].nextSkills[choiseIndex].statsIncrease[0]);
      rudos[rudoId].nextSkillsReady = false;
      emit LevelUp_Skill(rudoId,rudos[rudoId].level,rudos[rudoId].nextSkills[choiseIndex].statsIncrease[0]);
    }else{
      revert("Invalid choise Type");
    }
    rudos[rudoId].level ++;
  }

  function _createRudo2() public payable {
    uint256 _randomWord = 0;
    rudos.push();
    rudos[COUNTER].name = "fastRudo";
    rudos[COUNTER].level = 1;
    rudos[COUNTER].nature = randomNature(_randomWord);
    rudos[COUNTER].vitality = uint16(_randomWord % 50 + 1);
    rudos[COUNTER].strength = uint16(_randomWord/10 % 50 + 1);
    rudos[COUNTER].agility = uint16(_randomWord/100 % 50 + 1);
    rudos[COUNTER].velocity = uint16(_randomWord/1000 % 50 + 1);
    rudos[COUNTER].experience = 0;
    rudos[COUNTER].nextSkillsReady = false;
    _safeMint(msg.sender, COUNTER);
    emit NewRudo(msg.sender, COUNTER, rudos[COUNTER].name, rudos[COUNTER].vitality, rudos[COUNTER].strength, rudos[COUNTER].agility, rudos[COUNTER].velocity);
    COUNTER++;
  }

  // Creation
  function _createRudo(uint256 requestId, uint256 _randomWord) internal {
    rudos.push();
    rudos[COUNTER].name = requestToCharacterName[requestId];
    rudos[COUNTER].level = 1;
    rudos[COUNTER].nature = randomNature(_randomWord);
    rudos[COUNTER].vitality = uint16(_randomWord % 50 + 1);
    rudos[COUNTER].strength = uint16(_randomWord/10 % 50 + 1);
    rudos[COUNTER].agility = uint16(_randomWord/100 % 50 + 1);
    rudos[COUNTER].velocity = uint16(_randomWord/1000 % 50 + 1);
    rudos[COUNTER].experience = 0;
    rudos[COUNTER].nextSkillsReady = false;
    _safeMint(requestToSender[requestId], COUNTER);
    emit NewRudo(requestToSender[requestId], COUNTER, rudos[COUNTER].name, rudos[COUNTER].vitality, rudos[COUNTER].strength, rudos[COUNTER].agility, rudos[COUNTER].velocity);
    COUNTER++;
  }

  function randomNature(uint256 _randomWord) internal view returns (uint8){
    uint8 nature;
    uint256 _randomWord2 = _randomWord%naturesChance[3];
    for(uint8 i = 0; i < 4; i++){
      if(_randomWord2 < naturesChance[i]){
        nature = uint8(_randomWord % naturesCount[i]);
      }
    }
    return nature;
  }

  function setNext3Skills(uint256 id, LevelIncreaseChoise memory lvlinc1, LevelIncreaseChoise memory lvlinc2, LevelIncreaseChoise memory lvlinc3) payable public {
    rudos[id].nextSkills[0] = lvlinc1;
    rudos[id].nextSkills[1] = lvlinc2;
    rudos[id].nextSkills[2] = lvlinc3;
    rudos[id].nextSkillsReady = true;
  }
}