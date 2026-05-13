using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomEditor(typeof(NpcChatGraph))]
public class NpcChatGraphEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var graph = (NpcChatGraph)target;

        EditorGUILayout.Space();
        if (GUILayout.Button("Open Graph Editor"))
        {
            NodeEditorWindow.Open(graph);
        }

        if (GUILayout.Button("Add Chat Node"))
        {
            AddNode(graph);
        }
    }

    private static void AddNode(NpcChatGraph graph)
    {
        if (graph == null)
        {
            return;
        }

        Undo.RecordObject(graph, "Add chat node");

        NpcChatNode node = graph.AddNode<NpcChatNode>();
        node.name = $"NpcChatNode_{graph.nodes.Count}";
        node.position = new Vector2(240f * (graph.nodes.Count - 1), 0f);

        AssetDatabase.AddObjectToAsset(node, graph);
        Undo.RegisterCreatedObjectUndo(node, "Create chat node");

        if (graph.entryNode == null)
        {
            graph.entryNode = node;
        }

        EditorUtility.SetDirty(node);
        EditorUtility.SetDirty(graph);
        AssetDatabase.SaveAssets();
    }
}
