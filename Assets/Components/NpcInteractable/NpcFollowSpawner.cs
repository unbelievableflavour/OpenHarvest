using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NpcFollowSpawner : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;

        string followingId = GameState.Instance?.followingNpcId;
        if (string.IsNullOrWhiteSpace(followingId))
        {
            yield break;
        }

        bool specificInstanceTracked = !string.IsNullOrEmpty(GameState.Instance?.followingNpcInstanceId);

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
