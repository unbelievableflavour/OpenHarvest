using UnityEngine;
using System.Collections.Generic;

namespace HarvestDataTypes
{
    [CreateAssetMenu(fileName = "New Placeable Object", menuName = "Harvest/DataTypes/Placeable Object")]
    public class PlaceableObject : ScriptableObject
    {
        [Tooltip("Object ID should match the corresponding unlockable/item ID")]
        [ReadOnly]
        public string placeableObjectId;

        public GameObject prefab;
        public new string name;

        [TextArea]
        public string description = "";

        [Header("Buying")]
        public int buyPrice = 0;
        public List<string> stores = new List<string>();

        [Header("Placement")]
        [Tooltip("Allow this object to be placed on top of already placed objects.")]
        public bool canBePlacedOnPlacedObjects = false;

        [Tooltip("Allow this object to be placed on wall surfaces.")]
        public bool canBePlacedOnWall = false;

        private void OnValidate()
        {
            var scriptableObject = (ScriptableObject)this;
            placeableObjectId = scriptableObject.name;
        }
    }
}
