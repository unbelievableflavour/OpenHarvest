using OpenHarvest.EditorTools.ChatGraph;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NpcChatGraph))]
public class NpcChatGraphEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Graph Editor"))
        {
            NpcChatGraphEditorWindow.Open((NpcChatGraph)target);
        }

        DrawDefaultInspector();
    }
}
