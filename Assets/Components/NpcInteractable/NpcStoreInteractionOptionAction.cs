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
        Debug.Log("ApplyStoreConfiguration: " + instantiatedStore.name);
        if (instantiatedStore == null)
        {
            Debug.Log("ApplyStoreConfiguration: instantiatedStore is null");
            return;
        }
        Debug.Log("ApplyStoreConfiguration: instantiatedStore is not null");

        Store store = instantiatedStore.GetComponentInChildren<Store>();
        if (store != null)
        {
            Debug.Log("ApplyStoreConfiguration: store is not null");
            if (store.storeTitleLabel != null)
            {
                Debug.Log("ApplyStoreConfiguration: store.storeTitleLabel is not null");
                store.storeTitleLabel.ResetText(storeName);
            }

            if (store.storeDescriptionLabel != null)
            {
                Debug.Log("ApplyStoreConfiguration: store.storeDescriptionLabel is not null");
                store.storeDescriptionLabel.ResetText(storeDescription);
            }
        }

        StoreItemsLister storeItemsLister = instantiatedStore.GetComponentInChildren<StoreItemsLister>();
        if (storeItemsLister == null)
        {
            Debug.Log("ApplyStoreConfiguration: storeItemsLister is null");
            return;
        }

        Debug.Log("ApplyStoreConfiguration: storeItemsLister is not null");
        storeItemsLister.SetStoreNameFunctional(storeIdFunctional);
        storeItemsLister.SetStoreNameDecorational(storeIdDecorational);

        if (npc == null)
        {
            Debug.Log("ApplyStoreConfiguration: npc is null");
            return;
        }

        Debug.Log("ApplyStoreConfiguration: npc is not null");
        storeItemsLister.SetupStore(npc);
    }
}
