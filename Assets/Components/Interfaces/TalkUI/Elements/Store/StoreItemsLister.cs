using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Definitions;

public class StoreItemsLister : MonoBehaviour
{
    public ViewSwitcher viewSwitcher;
    public StoreDetailPage storeDetailPage;
    public GameObject itemRowPrefab;
    public GameObject animalRowPrefab;
    public GameObject decorationalButton;
    public GameObject functionalButton;

    public Transform scrollViewContent;

    public ItemPreviewer itemPreviewer;

    private string storeIdFunctional;
    private string storeIdDecorational;

    private List<HarvestDataTypes.Item> decorationalItemsInStore = new List<HarvestDataTypes.Item>();
    private List<HarvestDataTypes.Item> functionalItemsInStore = new List<HarvestDataTypes.Item>();

    public GameObject petInitialiser;

    List<string> animalsList = new List<string>() { "Chicken", "Cow", "Pig", "Sheep" };

    //You gotta run this before a store is operatable
    public void SetupStore(NPCController npc)
    {
        storeDetailPage.buyController.SetNPC(npc);
        GetStores();

        if (functionalItemsInStore.Count == 0)
        {
            fillDecorationalStore();
            return;
        }

        fillFunctionalStore();
    }

    private void GetStores()
    {
        if (storeIdDecorational != "")
        {
            decorationalItemsInStore = Definitions.itemStores[storeIdDecorational];
        }
        if (decorationalItemsInStore.Count == 0)
        {
            decorationalButton.SetActive(false);
        }

        if (storeIdFunctional != "")
        {
            functionalItemsInStore = Definitions.itemStores[storeIdFunctional];
        }

        if (functionalItemsInStore.Count == 0)
        {
            functionalButton.SetActive(false);
        }
    }

    public void fillDecorationalStore()
    {
        fillStore(decorationalItemsInStore, decorationalButton);
    }

    public void fillFunctionalStore()
    {
        fillStore(functionalItemsInStore, functionalButton);
    }

    private void fillStore(List<HarvestDataTypes.Item> selectedTabItemsInStore, GameObject currentButton)
    {

        ClearStore();
        if (selectedTabItemsInStore.Count == 0)
        {
            currentButton.SetActive(false);
            return;
        }

        foreach (HarvestDataTypes.Item item in selectedTabItemsInStore)
        {
            if (item == null)
            {
                continue;
            }

            GameObject row = Instantiate(animalsList.Contains(item.itemId) ? animalRowPrefab : itemRowPrefab) as GameObject;
            row.SetActive(true);
            var text = row.GetComponentInChildren<Text>();
            text.text = item.name;
            row.transform.SetParent(scrollViewContent, false);

            var animalRow = row.GetComponentInChildren<BuyAnimalController>();
            if (animalRow)
            {
                animalRow.SetItem(item);
                animalRow.SetItemLister(this);
                continue;
            }
            var itemRow = row.GetComponentInChildren<ItemRow>();
            itemRow.SetItem(item);
            itemRow.SetStoreItemsLister(this);
        }
    }

    public void RefreshStoreRows()
    {
        foreach (Transform item in scrollViewContent)
        {
            var animalRow = item.GetComponentInChildren<BuyAnimalController>();
            if (animalRow)
            {
                animalRow.Refresh();
                continue;
            }

            var itemRow = item.GetComponentInChildren<ItemRow>();
            itemRow.Refresh();
        }
    }

    public void LockStore(HarvestDataTypes.Item currentBoughtItem)
    {
        foreach (Transform item in scrollViewContent)
        {
            var itemRow = item.GetComponentInChildren<ItemRow>();
            itemRow.LockStoreItem(currentBoughtItem);
        }
    }

    public void UnlockStore()
    {
        foreach (Transform item in scrollViewContent)
        {
            var itemRow = item.GetComponentInChildren<ItemRow>();
            itemRow.Refresh();
        }
    }

    public void SetStoreNameFunctional(string newName) {
        this.storeIdFunctional = newName;
    }

    public void SetStoreNameDecorational(string newName)
    {
        this.storeIdDecorational = newName;
    }

    private void ClearStore()
    {
        foreach (Transform child in scrollViewContent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void InitialisePet(HarvestDataTypes.Item item)
    {
        petInitialiser.SetActive(true);
        petInitialiser.GetComponent<PetInitialiser>().SetItem(item);
    }
}
