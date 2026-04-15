#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HarvestDataTypes
{
    [CustomEditor(typeof(UnlockableDatabase))]
    public class UnlockableDatabaseEditor : Editor
    {
        private static readonly string[] IncludedFolders = new string[]
        {
            "Assets/ScriptableObjects/Unlockables/"
        };

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var database = (UnlockableDatabase)target;

            if (GUILayout.Button("Add all unlockables", GUILayout.Height(20)))
            {
                database.unlockables = new List<UnlockableDefinition>();

                foreach (string guid in AssetDatabase.FindAssets("", IncludedFolders))
                {
                    UnlockableDefinition unlockable = (UnlockableDefinition)AssetDatabase.LoadAssetAtPath(
                        AssetDatabase.GUIDToAssetPath(guid),
                        typeof(UnlockableDefinition)
                    );

                    if (unlockable == null)
                    {
                        continue;
                    }

                    database.unlockables.Add(unlockable);
                }

                EditorUtility.SetDirty(database);
            }
        }
    }
}
#endif
