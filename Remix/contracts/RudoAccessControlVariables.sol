// SPDX-License-Identifier: MIT

pragma solidity ^0.8.12;

import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/utils/Context.sol";

contract RudoAccessControlVariables is Ownable {
  address public gameController;
  address public rudoContract;
  address public rudoGameplay;
  uint256 public fee = 0 ether;

  constructor(){
     SetGameController(msg.sender);
  }

  function SetGameController(address addr) public onlyOwner{
      gameController = addr;
  }

  function SetFee(uint256 newfee) public{
    fee = newfee;
  }

  function SetRudoContract(address addr) public{
    rudoContract = addr;
  }

  function SetRudoGameplayContract(address addr) public{
    rudoGameplay = addr;
  }
}