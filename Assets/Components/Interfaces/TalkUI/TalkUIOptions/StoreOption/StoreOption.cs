using UnityEngine;

public class StoreOption : TalkUIOption
{
    [Header("Store settings")]
    public string storeIdFunctional;
    public string storeIdDecorational; 
    public string StoreName;
    [TextArea(3, 10)] public string StoreDescription;

    private GameObject instantiatedStore;

    public void SetInstantiatedStore(GameObject instantiatedStore)
    {
        this.instantiatedStore = instantiatedStore;
        SetStoreInformation();
    }

    public void SetStoreInformation()
    {
        if (string.IsNullOrEmpty(StoreName))
        {
            //This is done for the animalstore. remove when animal store is integrated in normal store
            return;
        }

        Store store = instantiatedStore.GetComponentInChildren<Store>();
        store.storeTitleLabel.ResetText(StoreName);
        store.storeDescriptionLabel.ResetText(StoreDescription);

        StoreItemsLister storeItemsLister = instantiatedStore.GetComponentInChildren<StoreItemsLister>();
        storeItemsLister.SetStoreNameFunctional(storeIdFunctional);
        storeItemsLister.SetStoreNameDecorational(storeIdDecorational);
        storeItemsLister.SetupStore(npc);
    }
}
