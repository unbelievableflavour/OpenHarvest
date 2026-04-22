using UnityEngine;

namespace HarvestDataTypes
{
    public class StoreProduct
    {
        public string id;
        public string displayName;
        public string description;
        public GameObject prefab;
        public int buyPrice;
        public int maximumTimesOwned;
        public string dependsOnId;
        public Item sourceItem;
        public PlaceableObject sourcePlaceableObject;
        public UnlockableDefinition sourceUnlockableDefinition;

        public static StoreProduct FromItem(Item item)
        {
            if (item == null)
            {
                return null;
            }

            return new StoreProduct
            {
                id = item.itemId,
                displayName = item.name,
                description = item.description,
                prefab = item.prefab,
                buyPrice = item.buyPrice,
                maximumTimesOwned = item.maximumTimesOwned,
                dependsOnId = item.DependsOnBeforeBuyingItem != null ? item.DependsOnBeforeBuyingItem.itemId : string.Empty,
                sourceItem = item,
                sourcePlaceableObject = null,
                sourceUnlockableDefinition = null
            };
        }

        public static StoreProduct FromPlaceableObject(PlaceableObject placeableObject)
        {
            if (placeableObject == null)
            {
                return null;
            }

            return new StoreProduct
            {
                id = placeableObject.placeableObjectId,
                displayName = placeableObject.name,
                description = placeableObject.description,
                prefab = null,
                buyPrice = placeableObject.buyPrice,
                maximumTimesOwned = 0,
                dependsOnId = string.Empty,
                sourceItem = null,
                sourcePlaceableObject = placeableObject,
                sourceUnlockableDefinition = null
            };
        }

        public static StoreProduct FromUnlockableDefinition(UnlockableDefinition unlockable)
        {
            if (unlockable == null)
            {
                return null;
            }

            string resolvedDisplayName = string.IsNullOrWhiteSpace(unlockable.displayName)
                ? unlockable.unlockableId
                : unlockable.displayName;

            return new StoreProduct
            {
                id = unlockable.unlockableId,
                displayName = resolvedDisplayName,
                description = unlockable.description,
                prefab = unlockable.linkedItem?.prefab,
                buyPrice = unlockable.buyPrice,
                maximumTimesOwned = unlockable.maximumTimesOwned,
                dependsOnId = unlockable.dependsOnId,
                sourceItem = unlockable.linkedItem,
                sourcePlaceableObject = null,
                sourceUnlockableDefinition = unlockable
            };
        }
    }
}
