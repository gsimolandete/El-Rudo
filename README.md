# El Rudo

El Rudo is an open source Unity and Web3 game prototype where players mint,
equip, and battle NFT characters called Rudos. The repository combines a Unity
client, Solidity contracts, Moralis cloud functions, and AWS Lambda combat
logic.

## Repository Layout

- `Unity/El Rudo`: Unity 2021.2.19f1 game client.
- `Remix/contracts`: Solidity contracts for Rudos, equipable items, gameplay,
  access control, and Chainlink VRF randomness.
- `Moralis/cloudfunctions`: cloud handlers for blockchain events and gameplay
  updates.
- `AWS/lambda-dotnetcore3.1`: serverless C# combat/backend logic.

## Current Status

The project is maintained as a playable prototype and reference implementation
for Unity-based Web3 game flows. The original demo targeted Polygon Mumbai,
which has since been deprecated; new deployments should use a currently
supported Polygon testnet and update RPC, faucet, and explorer settings before
publishing a build.

## Requirements

- Unity 2021.2.19f1.
- Google Chrome or another wallet-compatible browser.
- MetaMask or a compatible injected wallet.
- Solidity tooling compatible with `pragma solidity ^0.8.12`.
- OpenZeppelin and Chainlink contract dependencies for contract compilation.
- Optional: AWS SAM CLI and .NET tooling for the Lambda project.

## Gameplay Flow

1. Connect a wallet in the Unity WebGL build.
2. Open the shop and mint a Rudo.
3. Optionally mint weapons, shields, or pets.
4. Open the profile screen to inspect the Rudo and equip items.
5. Start a friendly duel by selecting your Rudo and entering an opponent Rudo id.

## Development Notes

- Do not commit wallet private keys, API keys, generated builds, debugger
  binaries, or local Unity cache folders.
- Smart contract changes should keep SPDX headers and be reviewed before
  deployment.
- Backend and contract changes should include clear manual test notes until
  automated test coverage is expanded.

## Releases

Historical releases include an early desktop build and a WebGL demo link. New
releases should document the target network, contract addresses, Unity version,
and manual verification steps.

## License

This project is licensed under the MIT License. See `LICENSE`.

## Maintenance Note

Repository hygiene, documentation, and a small maintainability refactor were
reviewed with OpenAI Codex to make future contributions easier to evaluate.
