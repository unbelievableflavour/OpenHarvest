using HarvestDataTypes;
using OVRSimpleJSON;
using System;
using UnityEngine;
using static Definitions;

public class ItemLoader : MonoBehaviour
{
    public void Start()
    {
        if(Definitions.ItemsAreLoaded)
        {
            Debug.Log("items are already loaded");
            return;
        }
        LoadAllItems();
        Definitions.ItemsAreLoaded = true;
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
        if (item.isUnlockable && !GameState.Instance.unlockables.ContainsKey(item.itemId))
        {
            GameState.Instance.unlockables[item.itemId] = 0;
        }

        if (!string.IsNullOrEmpty(item.type))
        {
            Definitions.itemsWithTypes[item.type].Add(item.itemId);
        }

        foreach (string storeId in item.stores)
        {
            try
            {
                Definitions.itemStores[storeId].Add(item);
            } catch(Exception e) {
                throw new Exception("Item store with ID: " + storeId + " does not exist, change the store ID for item: " + item.itemId);
            }
        }
    }
}
