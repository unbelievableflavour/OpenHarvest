using System.Collections.Generic;
using BNG;
using UnityEngine;

/// <summary>
/// Counts baubles hung on child <see cref="SnapZone"/>s under <see cref="hangLocationsRoot"/>.
/// Call <see cref="CheckStatus"/> after hang state changes (e.g. GrabAction on the Christmas tree, GrabbableUnityEvents, or SnapZone events).
/// When the count is satisfied and the quest is waiting on the matching world objective node, advances the quest graph.
/// </summary>
public class BaubleHangingLocations : MonoBehaviour
{
    [Tooltip("Typically the BaubleHangLocations transform whose children are snap slot roots.")]
    [SerializeField] private Transform hangLocationsRoot;

    [Min(1)]
    [SerializeField] private int minimumHungCount = 2;

    [Tooltip("Must match the Quest Graph asset's questId.")]
    [SerializeField] private string questId = "";

    [Tooltip("Must match QuestWorldObjectiveNode.objectiveKey on that graph.")]
    [SerializeField] private string objectiveKey = "";

    [Tooltip("If empty, any held item counts. Otherwise itemId must start with one of these prefixes (e.g. ChristmasBauble).")]
    [SerializeField] private List<string> allowedItemIdPrefixes = new List<string>();

    /// <summary>
    /// Invoked from tree GrabAction / inspector events after hang state changes.
    /// </summary>
    public void CheckStatus()
    {
        if (QuestRuntimeService.Instance == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(questId) || string.IsNullOrWhiteSpace(objectiveKey))
        {
            return;
        }

        if (GetHungCount() < minimumHungCount)
        {
            return;
        }

        QuestRuntimeService.Instance.TryCompleteWorldObjective(questId.Trim(), objectiveKey.Trim());
    }

    private int GetHungCount()
    {
        if (hangLocationsRoot == null)
        {
            return 0;
        }

        int count = 0;
        foreach (Transform child in hangLocationsRoot)
        {
            SnapZone slot = child != null ? child.GetComponent<SnapZone>() : null;
            if (slot == null || !slot.HeldItem)
            {
                continue;
            }

            if (!CountsAsValidBauble(slot.HeldItem))
            {
                continue;
            }

            count++;
        }

        return count;
    }

    private bool CountsAsValidBauble(Grabbable held)
    {
        if (held == null)
        {
            return false;
        }

        if (allowedItemIdPrefixes == null || allowedItemIdPrefixes.Count == 0)
        {
            return true;
        }

        HarvestDataTypes.Item item = Definitions.GetItemFromObject(held);
        if (item == null || string.IsNullOrEmpty(item.itemId))
        {
            return false;
        }

        string id = item.itemId;
        for (int i = 0; i < allowedItemIdPrefixes.Count; i++)
        {
            string prefix = allowedItemIdPrefixes[i];
            if (string.IsNullOrEmpty(prefix))
            {
                continue;
            }

            if (id.StartsWith(prefix.Trim()))
            {
                return true;
            }
        }

        return false;
    }
}
