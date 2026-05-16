using BNG;
using UnityEngine;

/// <summary>
/// Bridges NPC item handoff events into the gift system (both quest and generic gifts).
/// Add this on an NPC root that also has <see cref="NPCController"/> and
/// <see cref="NpcProximityInteractable"/> (same object or parent/child).
/// </summary>
public class NpcGiftBridge : MonoBehaviour
{
    private NPCController _npc;
    private NpcProximityInteractable _interactable;

    private bool _pendingGenericGift;
    private GameObject _pendingChatUIPrefab;
    private string _pendingResponseText;

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

    public void BeginGenericGift(GameObject chatUIPrefab, string responseText)
    {
        _pendingGenericGift = true;
        _pendingChatUIPrefab = chatUIPrefab;
        _pendingResponseText = responseText;

        if (_npc != null)
        {
            _npc.HoldOutHand();
        }
    }

    private void HandleNpcGrabbedItem(object sender, Grabbable grabbable)
    {
        if (_interactable == null)
        {
            return;
        }

        if (!TryResolveGiftData(grabbable, out string itemId, out int handedAmount, out ItemStack stack))
        {
            return;
        }

        if (QuestRuntimeService.Instance != null &&
            QuestRuntimeService.Instance.TrySubmitGift(_interactable, itemId, handedAmount, out int requiredAmount, out QuestNodeBase nextNode))
        {
            ConsumeAndDropOverflow(grabbable, stack, requiredAmount);

            if (_npc != null)
            {
                _npc.BackToIdle();
            }

            InteractionUIController interactionUI = FindFirstObjectByType<InteractionUIController>(FindObjectsInactive.Include);
            if (nextNode != null && !(nextNode is QuestWorldObjectiveNode))
            {
                QuestRuntimeService.Instance.RunNodeAction(nextNode, interactionUI, _interactable);
                return;
            }

            EventManager.Emit("interactionUIToMain");
            return;
        }

        HandleGenericGift(grabbable, stack);
    }

    private void HandleGenericGift(Grabbable grabbable, ItemStack stack)
    {
        if (!_pendingGenericGift)
        {
            return;
        }

        GameObject chatUIPrefab = _pendingChatUIPrefab;
        string responseText = _pendingResponseText;
        _pendingGenericGift = false;
        _pendingChatUIPrefab = null;
        _pendingResponseText = null;

        int amount = stack != null ? Mathf.Max(1, stack.GetStackSize()) : 1;
        ConsumeAndDropOverflow(grabbable, stack, amount);

        if (_npc != null)
        {
            _npc.BackToIdle();
        }

        if (!string.IsNullOrWhiteSpace(responseText) && chatUIPrefab != null)
        {
            InteractionUIController interactionUI = FindFirstObjectByType<InteractionUIController>(FindObjectsInactive.Include);
            if (interactionUI != null)
            {
                NPCController npc = _npc;
                interactionUI.ShowInstancedOptionContent(chatUIPrefab, _interactable, go =>
                {
                    NpcChatTreePresenter presenter = go.GetComponentInChildren<NpcChatTreePresenter>(true);
                    if (presenter != null)
                    {
                        presenter.BeginSingleLine(responseText, npc, showContinue: false);
                    }
                });
                return;
            }
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
