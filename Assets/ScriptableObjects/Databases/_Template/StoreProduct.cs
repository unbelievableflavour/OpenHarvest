using UnityEngine;

namespace HarvestDataTypes
{
    public class StoreProduct
    {
        public string id;
        public string ownedCountId;
        public string unlockableId;
        public string displayName;
        public string description;
        public GameObject prefab;
        public int buyPrice;
        public bool isUnlockable;
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

            string resolvedUnlockableId = item.itemId;
            string resolvedDependsOnId = item.DependsOnBeforeBuyingItem != null ? item.DependsOnBeforeBuyingItem.itemId : string.Empty;
            int resolvedMaximumTimesOwned = item.maximumTimesOwned;
            bool resolvedIsUnlockable = item.isUnlockable;

            return new StoreProduct
            {
                id = item.itemId,
                ownedCountId = resolvedUnlockableId,
                unlockableId = resolvedUnlockableId,
                displayName = item.name,
                description = item.description,
                prefab = item.prefab,
                buyPrice = item.buyPrice,
                isUnlockable = resolvedIsUnlockable,
                maximumTimesOwned = resolvedMaximumTimesOwned,
                dependsOnId = resolvedDependsOnId,
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
                ownedCountId = placeableObject.placeableObjectId,
                unlockableId = string.Empty,
                displayName = placeableObject.name,
                description = placeableObject.description,
                prefab = null,
                buyPrice = placeableObject.buyPrice,
                isUnlockable = true,
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
                ownedCountId = unlockable.unlockableId,
                unlockableId = unlockable.unlockableId,
                displayName = resolvedDisplayName,
                description = unlockable.description,
                prefab = null,
                buyPrice = unlockable.buyPrice,
                isUnlockable = true,
                maximumTimesOwned = unlockable.maximumTimesOwned,
                dependsOnId = unlockable.dependsOnId,
                sourceItem = null,
                sourcePlaceableObject = null,
                sourceUnlockableDefinition = unlockable
            };
        }
    }
}
