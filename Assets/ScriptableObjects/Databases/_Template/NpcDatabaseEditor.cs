#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NpcDatabase))]
public class NpcDatabaseEditor : Editor
{
    private static readonly string[] IncludedFolders = new[]
    {
        "Assets/NPCs/"
    };

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var database = (NpcDatabase)target;

        if (!GUILayout.Button("Add all NPCs", GUILayout.Height(20)))
        {
            return;
        }

        database.npcs = new List<NpcInteractableDefinition>();
        foreach (string guid in AssetDatabase.FindAssets("t:NpcInteractableDefinition", IncludedFolders))
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            NpcInteractableDefinition def = AssetDatabase.LoadAssetAtPath<NpcInteractableDefinition>(assetPath);
            if (def == null)
            {
                continue;
            }

            database.npcs.Add(def);
        }

        EditorUtility.SetDirty(database);
    }
}
#endif
