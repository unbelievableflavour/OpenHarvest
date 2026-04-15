using System.Collections.Generic;
using UnityEngine;

namespace HarvestDataTypes
{
    [CreateAssetMenu(fileName = "New Unlockable Database", menuName = "Harvest/Databases/Unlockables")]
    public class UnlockableDatabase : ScriptableObject
    {
        public List<UnlockableDefinition> unlockables;

        public UnlockableDefinition FindById(string id)
        {
            return unlockables.Find(unlockable => unlockable.unlockableId == id);
        }
    }
}
