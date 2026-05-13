---
name: create-openharvest-quests
description: Create new OpenHarvest quest graph assets and register them in the quest database. Use when asked to add quests, generate quest batches, or expand quest content in Assets/ScriptableObjects/Quests.
disable-model-invocation: true
---

# Create OpenHarvest Quests

## Goal

Add new quest graph assets under `Assets/ScriptableObjects/Quests/` with valid `.meta` files and `QuestDatabase` registration.

## Required Workflow

1. Read:
   - `Assets/ScriptableObjects/Databases/QuestDatabase.asset`
   - 2-3 existing quest assets in `Assets/ScriptableObjects/Quests/`
   - Node scripts in `Assets/Components/QuestsV2/` when needed
2. Design quests with varied structures and themes **using only existing node types** (see below).
3. Create `<QuestAssetName>.asset` and `<QuestAssetName>.asset.meta`.
4. Register each new quest GUID in `Assets/ScriptableObjects/Databases/QuestDatabase.asset`.
5. Verify all references and run a quick `git diff` check.

## Critical Pattern Rule

Do **not** always force the same quest flow pattern.

- The user explicitly does **not** want every quest to follow one fixed structure.
- Treat `chat -> gift -> chat -> finish` as only one option.
- Vary step composition across a batch (for example: chat chains, multi-gift relays, short 2-3 node quests, tutorial tours across multiple NPCs).

## Node types (strict — no custom nodes, zero world objectives)

For quests created with this skill (including batches and “generated” content), **only** use these three node classes from `Assets/Components/QuestsV2/`:

| Class | `m_Script` GUID | Use |
|-------|-----------------|-----|
| `QuestChatNode` | `cacdc6ed98f954da9be097f054ef5b26` | Dialogue / single-line chat |
| `QuestGiftNode` | `53b552c3ec1df4c6c998d906abcbb611` | Item hand-in step |
| `QuestFinishNode` | `427a07f6770dc4fc3848453c7855fdb6` | Rewards / end |

**Zero `QuestWorldObjectiveNode`:** Do **not** add world-objective nodes in agent-authored quest assets. The class exists for hand-built or separately specified work; this skill never uses GUID `411c58c55867492580b22286258a431a`.

**Do not:**

- Add or reference **new** `Quest*Node` C# types, other XNode node scripts, or mystery GUIDs.
- Invent node behavior that requires code the user did not ship.

**If the design needs a new node type** (behavior not covered by the three above): **stop and ask the user** before writing any new C# or asset that implies a new node.

## Quest Asset Rules

- File path: `Assets/ScriptableObjects/Quests/<QuestAssetName>.asset`
- Graph root:
  - `m_Script` guid for `QuestGraph`: `2c1ed10e13eee437faf7d47594e427e7`
  - Set `questId` (snake_case, unique)
  - Set `displayName` (human-readable)
  - `chatUIPrefab`: `{fileID: 1000, guid: 399b5284a57df47908efaec1c04b7cdb, type: 3}`
  - `entryNode` must point to first actionable node
- Per-node `m_Script` must be **one of** the three GUIDs in the table above only.
- Ensure flow links are coherent (`next` -> next node `inFlow`).
- **`QuestGiftNode`:** `giftPrompt` is NPC chat when offering the gift. Optional `tip` is the journal line (shown in **Quest UI** details).
- **`QuestChatNode`:** `body` is NPC chat. Optional `tip` is the same journal hint for the **Quest UI** details while that chat step is current.

## YAML serialization (ports)

- **Never** use inline flow maps for `ports` (for example `ports: {keys: [...], values: [...]}`). Commas inside `_typeQualifiedName` break Unity’s YAML parser and cause `Expected closing '}'` / XNode port deserialization errors.
- Always use the **expanded multiline** `ports:` layout (see `CleanupCrew.asset` or `HarborFishingRun.asset`): `keys:` list, then `values:` with full `_fieldName`, `_node`, `_typeQualifiedName` (split across lines), `connections`, etc.

## Meta File Rules

- Path must match quest filename: `<QuestAssetName>.asset.meta`
- Must contain one Unity meta block:
  - `fileFormatVersion: 2`
  - `guid: <32-char lowercase hex>`
  - `NativeFormatImporter`
  - `mainObjectFileID: 11400000`

## Database Registration Rules

Update `Assets/ScriptableObjects/Databases/QuestDatabase.asset`:

- Append entries under `quests:` as:
  - `- {fileID: 11400000, guid: <quest-guid>, type: 2}`
- Use GUIDs from the new quest `.meta` files.
- Keep existing entries intact.

## Content Guidance

- Vary NPC targets and quest purposes.
- Include tutorial-style quests when useful (for example, meeting multiple NPCs and learning roles).
- Balance short quests and longer multi-step quests in the same batch.
- Keep prompts concise and clear.

## Validation Checklist

- [ ] Each quest has matching `.asset` and `.asset.meta`.
- [ ] `questId` values are unique and readable.
- [ ] Every node uses only the three allowed `m_Script` GUIDs (no custom node types, **no** `QuestWorldObjectiveNode`).
- [ ] Step structures are varied across the batch.
- [ ] All new GUIDs are in `QuestDatabase.asset`.
- [ ] No unrelated files were modified.
