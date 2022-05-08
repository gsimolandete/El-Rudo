// SPDX-License-Identifier: MIT

pragma solidity ^0.8.12;

import "./RudoAccessControl.sol";
import "./Rudo.sol";

contract RudoGameplay is RudoAccessControl {
    constructor(address accessVariables)
        RudoAccessControl(accessVariables)
    {
        variables.SetRudoGameplayContract(address(this));
    }

    Rudo rudoContract;

    uint32 experiencePerGame = 2;

    uint32 baseRewardPerGame = 100; //1 = 0.0001$
    uint32 incrementPerElo = 1; //1 = 0.0001$

    int32 eloModifyBase = 20;
    int32 eloModifyPerDifference = 1; //1 = 0.1
    int32 maxEloDifference = 20;

    uint32 secondsPerGame = 28800;

    mapping (address => uint256) public rewards;

    event duelRequestEvent(uint256 challengerRudo, uint256 challengedRudo);

    function SetRudoContract(address addr) public {
        rudoContract = Rudo(addr);
    }
    
    function AddReward(address addr, uint256 ammount) internal onlyGameController{
        rewards[addr] += ammount;
    }

    function CalculateEloModify(int32 challengerRudoElo, int32 challengedRudoElo) internal view onlyGameController returns(int32){
        int32 eloDifference = challengedRudoElo - challengerRudoElo;
        eloDifference *= eloModifyPerDifference;
        if(eloDifference > maxEloDifference)    eloDifference = maxEloDifference;
        if(eloDifference < -maxEloDifference)    eloDifference = -maxEloDifference;

        return eloDifference;
    }

    function CalculateReward(uint32 challengedRudoElo) internal view onlyGameController returns(uint32){
        return baseRewardPerGame + incrementPerElo*challengedRudoElo;
    }

    function DuelRequest(uint256 challengerRudo, uint256 challengedRudo) external payable {
      require(rudoContract.ownerOf(challengerRudo) == msg.sender, "the challenger rudo is not owned by the caller address");
      require(rudoContract.GetRudoEnergy(challengerRudo) >= secondsPerGame, "the rudo doesn't have enough energy");
      require(challengerRudo != challengedRudo, "the two rudos are the same");
      emit duelRequestEvent(challengerRudo, challengedRudo);
    }

    function DuelComplete(bool challengerWon, uint256 challengerRudo, uint256 challengedRudo) external onlyGameController {
        require(rudoContract.GetRudoEnergy(challengerRudo) >= secondsPerGame, "the rudo doesn't have enough energy");
        require(challengerRudo != challengedRudo, "the two rudos are the same");

        int32 eloModify = CalculateEloModify(rudoContract.GetElo(challengerRudo),rudoContract.GetElo(challengerRudo));
        uint32 rudoEnergy = rudoContract.GetRudoEnergy(challengerRudo);
        if(challengerWon){
            uint32 reward = CalculateReward(uint32(rudoContract.GetElo(challengedRudo)));
            AddReward(rudoContract.ownerOf(challengerRudo), reward);
            rudoContract.ModifyElo(challengerRudo, eloModifyBase + eloModify);
            rudoContract.ModifyElo(challengedRudo, -(eloModifyBase - eloModify));
        }else{
            rudoContract.ModifyElo(challengerRudo, -(eloModifyBase - eloModify));
            rudoContract.ModifyElo(challengedRudo, eloModifyBase + eloModify);
        }
        rudoContract.SetFullEnergyTime(challengerRudo, uint32(block.timestamp) + (rudoContract.MAXENERGY() - rudoEnergy) + secondsPerGame);
        rudoContract.ModifyExperience(challengerRudo, experiencePerGame);
    }
}