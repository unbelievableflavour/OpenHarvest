using BNG;
using UnityEngine;

/// <summary>
/// Bridges NPC item handoff events into the quest runtime gift system.
/// Add this on an NPC root that also has <see cref="NPCController"/> and
/// <see cref="NpcProximityInteractable"/> (same object or parent/child).
/// </summary>
public class QuestNpcGiftBridge : MonoBehaviour
{
    private NPCController _npc;
    private NpcProximityInteractable _interactable;

    private void Awake()
    {
        _npc = GetComponentInParent<NPCController>();
        _interactable = GetComponentInChildren<NpcProximityInteractable>();
    }

    private void OnEnable()
    {
        if (_npc != null)
        {
            _npc.grabbedItem += HandleNpcGrabbedItem;
        }
    }

    private void OnDisable()
    {
        if (_npc != null)
        {
            _npc.grabbedItem -= HandleNpcGrabbedItem;
        }
    }

    private void HandleNpcGrabbedItem(object sender, Grabbable grabbable)
    {
        if (QuestRuntimeService.Instance == null || _interactable == null)
        {
            return;
        }

        if (!TryResolveGiftData(grabbable, out string itemId, out int handedAmount, out ItemStack stack))
        {
            return;
        }

        if (!QuestRuntimeService.Instance.TrySubmitGift(_interactable, itemId, handedAmount, out int requiredAmount))
        {
            return;
        }

        ConsumeAndDropOverflow(grabbable, stack, requiredAmount);

        if (_npc != null)
        {
            _npc.BackToIdle();
        }

        InteractionUIController interactionUI = FindFirstObjectByType<InteractionUIController>(FindObjectsInactive.Include);
        if (QuestRuntimeService.Instance.RunCurrentNodeForNpc(_interactable, interactionUI))
        {
            return;
        }

        EventManager.Emit("interactionUIToMain");
    }

    private static bool TryResolveGiftData(Grabbable grabbable, out string itemId, out int amount, out ItemStack stack)
    {
        itemId = null;
        amount = 1;
        stack = null;

        if (grabbable == null)
        {
            return false;
        }

        ItemInformation info = grabbable.GetComponent<ItemInformation>();
        if (info == null || info.item == null)
        {
            return false;
        }

        itemId = info.item.itemId;
        stack = grabbable.GetComponent<ItemStack>();
        amount = stack != null ? Mathf.Max(1, stack.GetStackSize()) : 1;
        return !string.IsNullOrWhiteSpace(itemId);
    }

    private static void ConsumeAndDropOverflow(Grabbable grabbable, ItemStack stack, int requiredAmount)
    {
        if (requiredAmount < 1)
        {
            requiredAmount = 1;
        }

        int handedAmount = stack != null ? Mathf.Max(1, stack.GetStackSize()) : 1;
        int overflow = handedAmount - requiredAmount;
        if (overflow <= 0)
        {
            // Exact amount: consume whole handed item.
            Object.Destroy(grabbable.gameObject);
            return;
        }

        // Consume required amount from handed stack and drop overflow as world item.
        HarvestDataTypes.Item item = Definitions.GetItemFromObject(grabbable);
        if (item != null && item.prefab != null)
        {
            GameObject dropped = Definitions.InstantiateItemNew(item.prefab);
            dropped.transform.position = grabbable.transform.position + Vector3.up * 0.1f;
            ItemStack droppedStack = dropped.GetComponent<ItemStack>();
            if (droppedStack != null)
            {
                droppedStack.SetStackSize(overflow);
            }
        }

        Object.Destroy(grabbable.gameObject);
    }
}
