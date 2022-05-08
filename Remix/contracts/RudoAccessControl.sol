// SPDX-License-Identifier: MIT

pragma solidity ^0.8.12;

import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/utils/Context.sol";
import "./RudoAccessControlVariables.sol";

abstract contract RudoAccessControl is Ownable {
  constructor(address addr){
    variables = RudoAccessControlVariables(addr);
  }
  
  RudoAccessControlVariables internal variables;

  modifier onlyRudoContract {
    require(msg.sender == variables.rudoContract(), "Caller is not rudo contract");
    _;
  }

  modifier onlyRudoGameplay {
    require(msg.sender == variables.rudoGameplay(), "Caller is not rudoGameplay contract");
    _;
  }

  modifier onlyGameController() {
    require(msg.sender == variables.gameController(), "Caller is not gameController wallet");
    _;
  }
  
  function withdraw() external payable onlyOwner {
    address payable _owner = payable(owner());
    _owner.transfer(address(this).balance);
  }

  function updateFee(uint256 _fee) external onlyOwner {
    variables.SetFee(_fee);
  }
}