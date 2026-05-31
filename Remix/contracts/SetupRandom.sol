// SPDX-License-Identifier: MIT

pragma solidity ^0.8.12;

import "@chainlink/contracts/src/v0.8/interfaces/VRFCoordinatorV2Interface.sol";
import "@chainlink/contracts/src/v0.8/VRFConsumerBaseV2.sol";

abstract contract SetupRandom is VRFConsumerBaseV2 {
    constructor(uint64 subscriptionId) VRFConsumerBaseV2(vrfCoordinator) {
        COORDINATOR = VRFCoordinatorV2Interface(vrfCoordinator);
        s_owner = msg.sender;
        s_subscriptionId = subscriptionId;
    }

    uint64 s_subscriptionId;
    address s_owner;
    VRFCoordinatorV2Interface COORDINATOR;

    address private constant POLYGON_MUMBAI_VRF_COORDINATOR = 0x6168499c0cFfCaCD319c818142124B7A15E857ab;
    bytes32 private constant POLYGON_MUMBAI_KEY_HASH = 0xd89b2bf150e3b9e13446986e571fb9cab24b13cea0a43ea20a6049a85cc807cc;

    address vrfCoordinator = POLYGON_MUMBAI_VRF_COORDINATOR;
    bytes32 keyHash = POLYGON_MUMBAI_KEY_HASH;

    mapping(uint256 => address) public requestToSender;
}
