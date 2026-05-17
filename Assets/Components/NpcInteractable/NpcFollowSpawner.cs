using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Place in every scene (e.g. on the SceneSwitcher object).
/// On scene start it checks whether GameState has a following NPC that needs to appear
/// in this scene, and spawns that NPC's prefab near the player if needed.
///
/// For unique NPCs (Jeff, Santa) the check is skipped when the NPC is already present.
/// For animals (Chicken, Cow…) where a specific instance is tracked, the origin instance
/// destroys itself in NPCNavAgent.Start() and this spawner always places a fresh copy.
/// The spawned NPC's own Start() then picks up the follow state automatically.
/// </summary>
public class NpcFollowSpawner : MonoBehaviour
{
    private IEnumerator Start()
    {
        // Wait one frame so NPCNavAgent.Start() has had a chance to destroy the origin
        // scene instance (Destroy is deferred to end-of-frame).
        yield return null;

        string followingId = GameState.Instance?.followingNpcId;
        if (string.IsNullOrWhiteSpace(followingId))
        {
            yield break;
        }

        bool specificInstanceTracked = !string.IsNullOrEmpty(GameState.Instance?.followingNpcInstanceId);

        // For unique NPCs, skip spawning when the NPC is already present and will
        // resume following via its own Start().
        if (!specificInstanceTracked && NpcAlreadyInScene(followingId))
        {
            yield break;
        }

        NpcInteractableDefinition def = DatabaseManager.Instance?.npcs?.FindById(followingId);
        if (def?.prefab == null)
        {
            yield break;
        }

        SpawnNearPlayer(def.prefab);
    }

    private static bool NpcAlreadyInScene(string npcId)
    {
        var all = FindObjectsByType<NPCNavAgent>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var nav in all)
        {
            if (nav.GetNpcId() == npcId)
            {
                return true;
            }
        }

        return false;
    }

    private static void SpawnNearPlayer(GameObject prefab)
    {
        Transform player = NPCNavAgent.ResolvePlayerFollowTarget();
        if (player == null)
        {
            return;
        }

        Vector3 spawnPos = player.position;
        if (NavMesh.SamplePosition(spawnPos, out var hit, 5f, NavMesh.AllAreas))
        {
            spawnPos = hit.position;
        }

        GameObject spawned = Instantiate(prefab, spawnPos, Quaternion.identity);

        string instanceId = GameState.Instance?.followingNpcInstanceId;
        if (!string.IsNullOrEmpty(instanceId))
        {
            spawned.GetComponentInChildren<DetermineModelByAge>(true)?.SetId(instanceId);
        }
    }
}
