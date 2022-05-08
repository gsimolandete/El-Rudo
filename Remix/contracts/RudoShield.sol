// SPDX-License-Identifier: MIT

pragma solidity ^0.8.12;

import "./RudoEquipable.sol";

contract RudoShield is RudoEquipable {
    constructor(address addr, string memory _name, string memory _symbol)
        RudoEquipable(addr,_name,_symbol)
    {

    }
}