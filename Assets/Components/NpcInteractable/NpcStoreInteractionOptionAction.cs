using UnityEngine;

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

        interactionUI.ShowInstancedOptionContent(storePrefab, instantiatedStore =>
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
            if (store.storeTitleLabel != null)
            {
                store.storeTitleLabel.ResetText(storeName);
            }

            if (store.storeDescriptionLabel != null)
            {
                store.storeDescriptionLabel.ResetText(storeDescription);
            }
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
}
