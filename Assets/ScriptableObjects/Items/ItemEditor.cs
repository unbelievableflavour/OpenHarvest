#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;

namespace HarvestDataTypes
{
    [CustomEditor(typeof(HarvestDataTypes.Item))]
    public class ItemEditor : Editor
    {
        private string dbLocation = "Assets/ScriptableObjects/Databases/ItemDatabase.asset";

        ItemDatabase db;
        private bool currentToggleValue;
        private bool lastToggleValue;

        public void OnEnable()
        {
            var so = (ScriptableObject)target;
            var item = (Item)target;
            
            item.itemId = so.name;

            db = (ItemDatabase)AssetDatabase.LoadAssetAtPath(dbLocation, typeof(ItemDatabase));
            currentToggleValue = db.items.Contains(item);
            lastToggleValue = currentToggleValue;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var item = (Item)target;

            currentToggleValue = GUILayout.Toggle(currentToggleValue, "Added to Database", GUILayout.Height(20));
            if (lastToggleValue != currentToggleValue)
            {
                if (currentToggleValue)
                {
                    db.items.Add(item);
                }
                else
                {
                    db.items.Remove(item);
                    
                }

                EditorUtility.SetDirty(db);
                lastToggleValue = currentToggleValue;
            }
        }
    }
}
#endif