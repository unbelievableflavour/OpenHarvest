#if (UNITY_EDITOR) 
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace HarvestDataTypes
{
    [CustomEditor(typeof(ItemDatabase))]
    public class ItemDatabaseEditor : Editor
    {
        private List<string> includedFolders = new List<string>() {
            "Assets/ScriptableObjects/Items/",
            "Assets/ScriptableObjects/Unlockables/", //Might wanna move these two out
            "Assets/ScriptableObjects/Animals/" //Might wanna move these two out
        };

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var database = (ItemDatabase)target;

            if (GUILayout.Button("Add all recipes", GUILayout.Height(20)))
            {
                foreach (string includedFolder in includedFolders)
                {
                    foreach (string s in AssetDatabase.FindAssets("", new string[] { includedFolder }))
                    {
                        Item item = (Item)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(s), typeof(Item));

                        if (item == null)
                        {
                            continue;
                        }

                        if (database.items.Contains(item))
                        {
                            continue;
                        }
                        database.items.Add(item);
                        EditorUtility.SetDirty(database);
                    }
                }
                
            }
        }
    }
}
#endif