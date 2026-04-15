#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;

namespace HarvestDataTypes
{
    [CustomEditor(typeof(Item))]
    public class ItemEditor : DatabaseConnectedObjectEditor
    {
        private string dbLocation = "Assets/ScriptableObjects/Databases/ItemDatabase.asset";

        public override void InitializeDB() {
            currentItem = (Item)target;
            db = (ItemDatabase)AssetDatabase.LoadAssetAtPath(dbLocation, typeof(ItemDatabase));
            dbList = db.items;
        }

        public void OnEnable()
        {
            base.OnEnable();
            
            var so = (ScriptableObject)target;
            currentItem.itemId = so.name;
        }
    }
}
#endif