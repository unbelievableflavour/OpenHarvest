using UnityEngine;
using System.Collections.Generic;

namespace HarvestDataTypes
{
    [CreateAssetMenu(fileName = "New Unlockable", menuName = "Harvest/DataTypes/Unlockable")]
    public class UnlockableDefinition : ScriptableObject
    {
        [Tooltip("Unlockable ID is set by filename")]
        [ReadOnly]
        public string unlockableId;

        [Header("Store")]
        public string displayName;

        [TextArea]
        public string description = "";
        public int buyPrice = 0;
        public List<string> stores = new List<string>();

        [Tooltip("Optional prerequisite unlockable ID")]
        public string dependsOnId;

        [Tooltip("0 means infinite")]
        public int maximumTimesOwned;

        [Tooltip("Optional: the Item this unlockable hands over as a physical prefab when purchased. Leave empty for pure unlockables (buildings, animals).")]
        public Item linkedItem;

        private void OnValidate()
        {
            var scriptableObject = (ScriptableObject)this;
            unlockableId = scriptableObject.name;
        }
    }
}
