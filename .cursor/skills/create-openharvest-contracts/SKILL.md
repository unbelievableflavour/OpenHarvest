---
name: create-openharvest-contracts
description: Create new OpenHarvest contract ScriptableObject assets and register them in the contract database. Use when asked to add one or more contracts, generate contract batches, or expand contract content in Assets/ScriptableObjects/Contracts.
disable-model-invocation: true
---

# Create OpenHarvest Contracts

## Goal

Add new contract assets under `Assets/ScriptableObjects/Contracts/` in Unity YAML format, with valid `.meta` files and database registration.

## Required Workflow

1. Read these files first:
   - `Assets/ScriptableObjects/Contracts/_Template/Contract.cs`
   - `Assets/ScriptableObjects/Databases/ContractDatabase.asset`
   - 1-2 existing contract assets in `Assets/ScriptableObjects/Contracts/`
2. Define contract concepts with varied themes and requirements (avoid repetitive content unless user asks).
3. For each new contract:
   - Create `<ContractId>.asset`
   - Create `<ContractId>.asset.meta`
4. Register each new contract GUID in `Assets/ScriptableObjects/Databases/ContractDatabase.asset` under `contracts:`.
5. Verify consistency and run a quick `git diff` check.

## Contract Asset Rules

For each `.asset`:

- File path must be `Assets/ScriptableObjects/Contracts/<ContractId>.asset`.
- Set `m_Name` to the same `<ContractId>`.
- Set `contractId` to the same `<ContractId>`.
- `name` is human-readable display text.
- `description` should be concise and specific.
- `requirements` must include `amount` and `item` references.
- Include `rewardGold` and `rewardItems: []`.

Keep these three aligned exactly:

- filename stem
- `m_Name`
- `contractId`

## Meta File Rules

For each `.meta`:

- Path must match asset filename: `<ContractId>.asset.meta`.
- Must contain a single valid Unity meta block:
  - `fileFormatVersion: 2`
  - `guid: <32-char lowercase hex>`
  - `NativeFormatImporter` block
- Do not duplicate meta blocks in one file.

## Database Registration Rules

Update `Assets/ScriptableObjects/Databases/ContractDatabase.asset`:

- Add one entry per new contract under `contracts:`
- Format:
  - `- {fileID: 11400000, guid: <contract-guid>, type: 2}`
- Use the GUID from that contract's `.meta` file.
- Preserve existing entries and formatting style.

## Content Guidance

- Prefer variety across categories (fish, farming, tools, recycling, festival logistics, mining, cooking).
- Vary requirement counts and number of required items.
- Keep reward scaling sensible to difficulty.
- Reuse existing item GUIDs from known valid contract assets unless user requests new items.

## Validation Checklist

- [ ] Every new contract has both `.asset` and `.asset.meta`.
- [ ] Filename, `m_Name`, and `contractId` match.
- [ ] `.meta` has exactly one GUID block.
- [ ] All new GUIDs are present in `ContractDatabase.asset`.
- [ ] No unrelated files were modified.
