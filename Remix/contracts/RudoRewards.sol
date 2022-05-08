// SPDX-License-Identifier: MIT

pragma solidity ^0.8.12;

contract RudoRewards {
    constructor(){

    }

    address rudoGameplayAddress;

    mapping (address => uint256) public rewards;

    function AddReward(uint256 ammount) public {
        require(msg.sender == address(rudoGameplayAddress));
        rewards[msg.sender] += ammount;
    }

}