// SPDX-License-Identifier: MIT

pragma solidity ^0.8.12;

import "@openzeppelin/contracts/token/ERC721/ERC721.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/token/ERC721/extensions/ERC721Enumerable.sol";
import "@openzeppelin/contracts/utils/Strings.sol";
import "@chainlink/contracts/src/v0.8/interfaces/LinkTokenInterface.sol";
import "@chainlink/contracts/src/v0.8/interfaces/VRFCoordinatorV2Interface.sol";
import "@chainlink/contracts/src/v0.8/VRFConsumerBaseV2.sol";
import "./SetupRandom.sol";
import "./RudoGameplay.sol";
import "./RudoWeapon.sol";
import "./RudoPet.sol";
import "./RudoShield.sol";

contract Rudo is ERC721Enumerable, SetupRandom, RudoAccessControl {
  constructor(address accessVariables, address weapons, address pets, address shields,string memory _name, string memory _symbol)
    SetupRandom(198)
    ERC721(_name, _symbol)
    RudoAccessControl(accessVariables)
  {
    variables.SetRudoContract(address(this));
    SetWeaponContract(weapons);
    SetPetContract(pets);
    SetShieldContract(shields);
  }

  uint32 callbackGasLimit = 1800000; //careXD
  uint16 requestConfirmations = 3;
  mapping(uint256 => string) public requestToCharacterName;//try to reduce mappings by making them the same value type?
  //LEVELS
  uint32[] experienceRequiered = [4,6,8,10,12,14,16,18,20,22,24,26,28,30,32,34,36,38,40];
  //NATURES
  uint16[4] naturesChance = [100,1500,5000,10000];
  uint16[4] maxNatures = [1000,2000,3000,4000];
  uint16[4] naturesCount = [5,10,15,20];
  //RUDOS
  RudoStruct[] public rudos;
  //Equipable COntracts
  RudoPet petContract;
  RudoShield shieldContract;
  RudoWeapon weaponContract;
  //ENERGY
  uint32 public MAXENERGY = 86400;


  struct RudoStruct {
    string name;
    uint16 level;
    uint32 experience;//uint32?
    uint16 nature;
    RudoStats stats;
    uint16[] skills;
    bool nextSkillsReady; //implement
    LevelIncreaseChoise[3] nextSkills;
    int32 elo;
    EquipableSlot pet;
    EquipableSlot shield;
    uint256[] weapons;
    RudoEnergy energy;
  }

  struct EquipableSlot {
    bool slotUsed;
    uint256 Id;
  }

  struct RudoStats{
    uint16 vitality;
    uint16 strength;
    uint16 agility;
    uint16 velocity;
  }

  struct RudoEnergy {
    uint32 fullEnergyTime;
  }

  struct LevelIncreaseChoise {
    LvlIncType choiseType; //0 == stats, 1 == skill, 
    uint16[4] statsIncrease;
  }

  enum LvlIncType{ SKILL, STATS }

  event NewRudo (address owner, uint256 id, string name, uint16 vitality, uint16 strength, uint16 agility, uint16 velocity);
  event GainExp (uint256 id, uint32 experience);
  event LevelUp_Skill (uint256 id, uint16 level, uint16 skillId);
  event LevelUp_Stats (uint256 id, uint16 level, uint16 vitality, uint16 strength, uint16 agility, uint16 velocity);
  //EQUIPABLES
  event SetShieldEvent(uint256 rudoId, uint256 equipableId);
  event SetPetEvent(uint256 rudoId, uint256 equipableId);
  event SetWeaponEvent(uint256 rudoId, uint256 equipableId);
  event RemoveShieldEvent(uint256 rudoId);
  event RemovePetEvent(uint256 rudoId);
  event RemoveWeaponEvent(uint256 rudoId, uint256 equipableId);
  event Log (string logMessage, uint256 logNumber);

  function GetElo(uint256 id) public view returns (int32){
    return rudos[id].elo;
  }

  function GetWeapons(uint256 id) public view returns (uint256[] memory){
    return rudos[id].weapons;
  }

  function GetWeaponsLength(uint256 id) public view returns (uint256){
    return rudos[id].weapons.length;
  }

  function GetSkills(uint256 id) public view returns (uint16[] memory){
    return rudos[id].skills;
  }

  function GetLevelIncreases(uint256 id) public view returns (LevelIncreaseChoise[3] memory){
    return rudos[id].nextSkills;
  }

  function GetRudoEnergy(uint256 rudoId) public view returns(uint32){
    uint time = block.timestamp;
    time = rudos[rudoId].energy.fullEnergyTime < time ? 0 : rudos[rudoId].energy.fullEnergyTime - time;
    if(time >= MAXENERGY)
      return 0;
    else
      return MAXENERGY-uint32(time);
  }

  function SetPetContract(address addr) public onlyOwner{
    petContract = RudoPet(addr);
  }

  function SetWeaponContract(address addr) public onlyOwner{
    weaponContract = RudoWeapon(addr);
  }

  function SetShieldContract(address addr) public onlyOwner{
    shieldContract = RudoShield(addr);
  }

  function setNext3Skills(uint256 id, LevelIncreaseChoise memory lvlinc1, LevelIncreaseChoise memory lvlinc2, LevelIncreaseChoise memory lvlinc3) payable public {
    rudos[id].nextSkills[0] = lvlinc1;
    rudos[id].nextSkills[1] = lvlinc2;
    rudos[id].nextSkills[2] = lvlinc3;
    rudos[id].nextSkillsReady = true;
  }

  function SetFullEnergyTime(uint256 rudoId, uint32 fullEnergyTime) external onlyRudoGameplay {
    rudos[rudoId].energy.fullEnergyTime = fullEnergyTime;
  }

  function SetWeapon(uint256 rudoId, uint256 weaponId) public {
    require(ownerOf(rudoId) == msg.sender, "Caller is not the owner of the rudo");
    require(weaponContract.ownerOf(weaponId) == msg.sender, "Caller is not the owner of the weapon");
    require(!weaponContract.IsEquiped(weaponId), "Weapon is already equiped");

    rudos[rudoId].weapons.push(weaponId);
    weaponContract.Equip(weaponId);
    emit SetWeaponEvent(rudoId,weaponId);
  }

  function RemoveWeapon(uint256 rudoId, uint256 weaponId) public {
    require(ownerOf(rudoId) == msg.sender, "Caller is not the owner of the rudo");
    require(weaponContract.IsEquiped(weaponId), "Weapon is not equiped");
    for(uint i = 0; i<rudos[rudoId].weapons.length;i++){
      if(rudos[rudoId].weapons[i]==weaponId){
        rudos[rudoId].weapons[i] = rudos[rudoId].weapons[rudos[rudoId].weapons.length-1];
        rudos[rudoId].weapons.pop();
        weaponContract.Unequip(weaponId);
      }
    }
    emit RemoveWeaponEvent(rudoId,weaponId);
  }

  function SetShield(uint256 rudoId, uint256 shieldId) public {
    require(ownerOf(rudoId) == msg.sender, "Caller is not the owner of the rudo");
    require(shieldContract.ownerOf(shieldId) == msg.sender, "Caller is not the owner of the shield");
    require(!shieldContract.IsEquiped(shieldId), "Shield is already equiped");

    if(rudos[rudoId].shield.slotUsed)
      RemoveShield(shieldId);

    rudos[rudoId].shield.Id = shieldId;
    rudos[rudoId].shield.slotUsed = true;
    shieldContract.Equip(shieldId);
    emit SetShieldEvent(rudoId,shieldId);
  }

  function RemoveShield(uint256 rudoId) public {
    require(ownerOf(rudoId) == msg.sender, "Caller is not the owner of the rudo");
    require(rudos[rudoId].shield.slotUsed, "No shield equiped");
    shieldContract.Unequip(rudos[rudoId].shield.Id);
    rudos[rudoId].shield.slotUsed = false;
    emit RemoveShieldEvent(rudoId);
  }

  function SetPet(uint256 rudoId, uint256 petId) public {
    require(ownerOf(rudoId) == msg.sender, "Caller is not the owner of the rudo");
    require(petContract.ownerOf(petId) == msg.sender, "Caller is not the owner of the pet");
    require(!petContract.IsEquiped(petId), "Pet is already equiped");

    if(rudos[rudoId].pet.slotUsed)
      RemovePet(petId);

    rudos[rudoId].pet.Id = petId;
    rudos[rudoId].pet.slotUsed = true;
    petContract.Equip(petId);
    emit SetPetEvent(rudoId, petId);
  }

  function RemovePet(uint256 rudoId) public {
    require(ownerOf(rudoId) == msg.sender, "Caller is not the owner of the rudo");
    require(rudos[rudoId].pet.slotUsed, "Pet is not equiped");
    petContract.Unequip(rudos[rudoId].pet.Id);
    rudos[rudoId].pet.slotUsed = false;
    emit RemovePetEvent(rudoId);
  }

  function ModifyElo(uint256 rudoId, int32 eloModify) public onlyRudoGameplay {
    rudos[rudoId].elo += eloModify;
    if(rudos[rudoId].elo < 0) rudos[rudoId].elo = 0;
  }

  function ModifyExperience(uint256 rudoId, uint32 experienceModify) external onlyRudoGameplay {
    rudos[rudoId].experience += experienceModify;
  }

  function updateCallbackGasLimit(uint32 _callbackGasLimit) external onlyOwner{
    callbackGasLimit = _callbackGasLimit;
  }

  function fulfillRandomWords(
    uint256 requestId,
    uint256[] memory randomWords
  ) internal override {
    _createRudo(requestToSender[requestId], requestToCharacterName[requestId], randomWords[0]);
  }

  function RequestRandomRudo(string memory _name) public payable {
    require(msg.value >= variables.fee());
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
      rudos[rudoId].stats.vitality += rudos[rudoId].nextSkills[choiseIndex].statsIncrease[0];
      rudos[rudoId].stats.strength += rudos[rudoId].nextSkills[choiseIndex].statsIncrease[1];
      rudos[rudoId].stats.agility += rudos[rudoId].nextSkills[choiseIndex].statsIncrease[2];
      rudos[rudoId].stats.velocity += rudos[rudoId].nextSkills[choiseIndex].statsIncrease[3];
      rudos[rudoId].nextSkillsReady = false;
      emit LevelUp_Stats(rudoId,rudos[rudoId].level,rudos[rudoId].stats.vitality,rudos[rudoId].stats.strength,rudos[rudoId].stats.agility,rudos[rudoId].stats.velocity);
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
    _createRudo(msg.sender, "fastRudo",block.timestamp);
  }

  function _createRudo3() public payable {
    _createRudo(msg.sender, "fastRudo",block.timestamp);
    require(false);
  }

  // Creation
  function _createRudo(address to, string memory name, uint256 _randomWord) internal {
    uint256 COUNTER = rudos.length;
    rudos.push();
    rudos[COUNTER].name = name;
    rudos[COUNTER].level = 1;
    rudos[COUNTER].nature = randomNature(_randomWord);
    rudos[COUNTER].stats.vitality = uint16(_randomWord % 50 + 1);
    rudos[COUNTER].stats.strength = uint16(_randomWord/10 % 50 + 1);
    rudos[COUNTER].stats.agility = uint16(_randomWord/100 % 50 + 1);
    rudos[COUNTER].stats.velocity = uint16(_randomWord/1000 % 50 + 1);
    rudos[COUNTER].experience = 0;
    rudos[COUNTER].elo = 0;
    rudos[COUNTER].nextSkillsReady = false;
    _safeMint(to, COUNTER);
    emit NewRudo(to, COUNTER, rudos[COUNTER].name, rudos[COUNTER].stats.vitality, rudos[COUNTER].stats.strength, rudos[COUNTER].stats.agility, rudos[COUNTER].stats.velocity);
    COUNTER++;
  }

  function randomNature(uint256 _randomWord) internal view returns (uint16){
    uint16 nature;
    uint256 _randomWord2 = _randomWord%naturesChance[3];
    for(uint8 i = 0; i < 4; i++){
      if(_randomWord2 < naturesChance[i]){
        nature = uint16(_randomWord % naturesCount[i]);
      }
    }
    return nature;
  }
}