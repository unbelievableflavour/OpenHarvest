using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(
    fileName = "Npc Store Option Action",
    menuName = "OpenHarvest/NPC/Interaction Option Actions/Store",
    order = 3)]
public class NpcStoreInteractionOptionAction : NpcInteractionOptionAction
{
    [Header("Store settings")]
    [SerializeField] private string storeIdFunctional;
    [SerializeField] private string storeIdDecorational;
    [SerializeField] private string storeName;
    [TextArea(3, 10)] [SerializeField] private string storeDescription;

    [Tooltip("Store prefab instantiated under the interaction view when this option is clicked.")]
    [SerializeField] private GameObject storePrefab;

    public override bool IsValid(NpcInteractionOption option)
    {
        if (option == null || string.IsNullOrWhiteSpace(option.displayName) || storePrefab == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(storeName))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(storeIdFunctional) && string.IsNullOrWhiteSpace(storeIdDecorational))
        {
            return false;
        }

        return true;
    }

    public override void Execute(
        InteractionUIController interactionUI,
        NpcProximityInteractable interactable,
        NpcInteractionOption option)
    {
        if (interactionUI == null || storePrefab == null)
        {
            return;
        }

        NPCController npc = interactable != null
            ? interactable.GetComponentInParent<NPCController>()
            : null;

        interactionUI.ShowInstancedOptionContent(storePrefab, interactable, instantiatedStore =>
        {
            ApplyStoreConfiguration(instantiatedStore, npc);
        });
    }

    private void ApplyStoreConfiguration(GameObject instantiatedStore, NPCController npc)
    {
        if (instantiatedStore == null)
        {
            return;
        }

        Store store = instantiatedStore.GetComponentInChildren<Store>();
        if (store != null)
        {
            ApplyStoreLabel(store.storeTitleLabel, storeName);
            ApplyStoreLabel(store.storeDescriptionLabel, storeDescription);
        }

        StoreItemsLister storeItemsLister = instantiatedStore.GetComponentInChildren<StoreItemsLister>();
        if (storeItemsLister == null)
        {
            return;
        }

        storeItemsLister.SetStoreNameFunctional(storeIdFunctional);
        storeItemsLister.SetStoreNameDecorational(storeIdDecorational);

        if (npc == null)
        {
            return;
        }

        storeItemsLister.SetupStore(npc);
    }

    private static void ApplyStoreLabel(AutoType label, string value)
    {
        if (label == null)
        {
            return;
        }

        // AutoType may already be animating placeholder text from prefab.
        // Stop that coroutine so stale characters are not appended afterward.
        label.StopAllCoroutines();
        label.ResetText(value);

        Text labelText = label.GetComponent<Text>();
        if (labelText == null)
        {
            return;
        }

        label.Refresh();
    }
}
