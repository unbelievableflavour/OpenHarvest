using System.Collections.Generic;
using UnityEngine;

namespace HarvestDataTypes {

    [CreateAssetMenu(fileName = "New Item", menuName = "Harvest/DataTypes/Item")]
    public class Item : ScriptableObject
    {
        [Tooltip("Item ID is set by filename")]
        [ReadOnly]
        public string itemId;
        public GameObject prefab;
        public new string name;

        [TextArea]
        public string description = "";

        [Header("Selling")]
        public bool isSellable;
        public int sellPrice = 0;

        [Header("Buying")]
        public int buyPrice = 0;
        public List<string> stores;
        public string DependsOnBeforeBuying;

        [Header("Unlockable")]
        public bool isUnlockable;

        [Tooltip("0 means infinite")]
        public int maximumTimesOwned;

        [Header("Item of the week")]
        public string type;
    }
}