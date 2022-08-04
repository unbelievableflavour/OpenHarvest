using HarvestDataTypes;
using OVRSimpleJSON;
using System;
using UnityEngine;
using static Definitions;

public class ItemLoader : MonoBehaviour
{
    public void Start()
    {
        if(Definitions.ItemsWithInformation.Count != 0)
        {
            Debug.Log("item list is already filled");
            return;
        }
        LoadAllItems();
    }

    public void LoadAllItems()
    {
        foreach (HarvestDataTypes.Item item in DatabaseManager.Instance.items.items)
        {
            LoadItem(item);
        }
    }

    private void LoadItem(HarvestDataTypes.Item item)
    {
        string itemId = item.itemId;

        Item newItem = new Item();
        newItem.itemId = itemId;
        newItem.name = item.name;

        if (item.prefab != null)
        {
            newItem.prefabFileName = item.prefab.transform.name;
        }
        
        newItem.description = item.description;
        newItem.DependsOnBeforeBuying = item.DependsOnBeforeBuying;
        newItem.sellPrice = item.sellPrice;
        newItem.buyPrice = item.buyPrice;

        if (item.maximumTimesOwned != 0)
        {
            newItem.maximumTimesOwned = item.maximumTimesOwned;
        }

        if (item.isUnlockable)
        {
            GameState.unlockables[itemId] = 0;
        }

        ItemsWithInformation.Add(itemId, newItem);

        if (!string.IsNullOrEmpty(item.type))
        {
            Definitions.itemsWithTypes[item.type].Add(itemId);
        }

        foreach (string storeId in item.stores)
        {
            try
            {
                Definitions.itemStores[storeId].Add(itemId);
            } catch(Exception e) {
                throw new Exception("Item store with ID: " + storeId + " does not exist, change the store ID for item: " + itemId);
            }
        }
    }

    public void LoadItemInStores(JSONArray stores, string itemId)
    {
        foreach (JSONNode store in stores)
        {
            Definitions.itemStores[store["name"]].Add(itemId);
        }
    }
}
