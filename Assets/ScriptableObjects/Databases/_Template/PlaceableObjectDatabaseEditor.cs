#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HarvestDataTypes
{
    [CustomEditor(typeof(PlaceableObjectDatabase))]
    public class PlaceableObjectDatabaseEditor : Editor
    {
        private static readonly string[] IncludedFolders = new string[]
        {
            "Assets/PlaceableObjects/"
        };

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var database = (PlaceableObjectDatabase)target;

            if (GUILayout.Button("Add all placeable objects", GUILayout.Height(20)))
            {
                database.objectsData = new List<PlaceableObject>();

                foreach (string guid in AssetDatabase.FindAssets("", IncludedFolders))
                {
                    PlaceableObject placeableObject = (PlaceableObject)AssetDatabase.LoadAssetAtPath(
                        AssetDatabase.GUIDToAssetPath(guid),
                        typeof(PlaceableObject)
                    );

                    if (placeableObject == null)
                    {
                        continue;
                    }

                    database.objectsData.Add(placeableObject);
                }

                EditorUtility.SetDirty(database);
            }
        }
    }
}
#endif
