#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuestDatabase))]
public class QuestDatabaseEditor : Editor
{
    private static readonly string[] IncludedFolders = new[]
    {
        "Assets/ScriptableObjects/Quests/"
    };

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var database = (QuestDatabase)target;

        if (!GUILayout.Button("Add all quests", GUILayout.Height(20)))
        {
            return;
        }

        database.quests = new List<QuestGraph>();
        foreach (string guid in AssetDatabase.FindAssets("t:QuestGraph", IncludedFolders))
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            QuestGraph quest = AssetDatabase.LoadAssetAtPath<QuestGraph>(assetPath);
            if (quest == null)
            {
                continue;
            }

            database.quests.Add(quest);
        }

        EditorUtility.SetDirty(database);
    }
}
#endif
