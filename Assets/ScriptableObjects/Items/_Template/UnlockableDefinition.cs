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
        public GameObject previewPrefab;

        [Tooltip("Optional prerequisite unlockable ID")]
        public string dependsOnId;

        [Tooltip("0 means infinite")]
        public int maximumTimesOwned;

        private void OnValidate()
        {
            var scriptableObject = (ScriptableObject)this;
            unlockableId = scriptableObject.name;
        }
    }
}
