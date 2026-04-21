using HarvestDataTypes;
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

        if (DatabaseManager.Instance.placeableObjects != null && DatabaseManager.Instance.placeableObjects.objectsData != null)
        {
            foreach (HarvestDataTypes.PlaceableObject placeableObject in DatabaseManager.Instance.placeableObjects.objectsData)
            {
                LoadPlaceableObject(placeableObject);
            }
        }

        if (DatabaseManager.Instance.unlockables != null && DatabaseManager.Instance.unlockables.unlockables != null)
        {
            foreach (HarvestDataTypes.UnlockableDefinition unlockable in DatabaseManager.Instance.unlockables.unlockables)
            {
                LoadUnlockable(unlockable);
            }
        }
    }

    private void LoadItem(HarvestDataTypes.Item item)
    {
        if (!string.IsNullOrEmpty(item.type))
        {
            Definitions.itemsWithTypes[item.type].Add(item.itemId);
        }

        HarvestDataTypes.StoreProduct storeProduct = HarvestDataTypes.StoreProduct.FromItem(item);
        foreach (string storeId in item.stores)
        {
            try
            {
                Definitions.itemStores[storeId].Add(storeProduct);
            } catch(Exception e) {
                throw new Exception("Item store with ID: " + storeId + " does not exist, change the store ID for item: " + item.itemId);
            }
        }
    }

    private void LoadPlaceableObject(HarvestDataTypes.PlaceableObject placeableObject)
    {
        if (placeableObject == null || placeableObject.stores == null)
        {
            return;
        }

        HarvestDataTypes.StoreProduct storeProduct = HarvestDataTypes.StoreProduct.FromPlaceableObject(placeableObject);
        foreach (string storeId in placeableObject.stores)
        {
            try
            {
                Definitions.itemStores[storeId].Add(storeProduct);
            }
            catch (Exception)
            {
                throw new Exception("Item store with ID: " + storeId + " does not exist, change the store ID for placeable object: " + placeableObject.placeableObjectId);
            }
        }
    }

    private void LoadUnlockable(HarvestDataTypes.UnlockableDefinition unlockable)
    {
        if (unlockable == null || unlockable.stores == null)
        {
            return;
        }

        HarvestDataTypes.StoreProduct storeProduct = HarvestDataTypes.StoreProduct.FromUnlockableDefinition(unlockable);
        foreach (string storeId in unlockable.stores)
        {
            try
            {
                Definitions.itemStores[storeId].Add(storeProduct);
            }
            catch (Exception)
            {
                throw new Exception("Item store with ID: " + storeId + " does not exist, change the store ID for unlockable: " + unlockable.unlockableId);
            }
        }
    }
}
