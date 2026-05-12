using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomEditor(typeof(QuestGraph))]
public class QuestGraphEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var graph = (QuestGraph)target;

        EditorGUILayout.Space();
        if (GUILayout.Button("Open Graph Editor"))
        {
            NodeEditorWindow.Open(graph);
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Add Chat Node"))
        {
            AddNode<QuestChatNode>(graph);
        }

        if (GUILayout.Button("Add Gift Node"))
        {
            AddNode<QuestGiftNode>(graph);
        }

        if (GUILayout.Button("Add Finish Node"))
        {
            AddNode<QuestFinishNode>(graph);
        }

        if (GUILayout.Button("Add World Objective Node"))
        {
            AddNode<QuestWorldObjectiveNode>(graph);
        }
    }

    private static void AddNode<TNode>(QuestGraph graph) where TNode : QuestNodeBase
    {
        if (graph == null)
        {
            return;
        }

        Undo.RecordObject(graph, "Add quest node");

        TNode node = graph.AddNode<TNode>();
        if (node == null)
        {
            return;
        }

        node.name = $"QuestNode_{graph.nodes.Count}";
        node.position = new Vector2(260f * (graph.nodes.Count - 1), 0f);

        AssetDatabase.AddObjectToAsset(node, graph);
        Undo.RegisterCreatedObjectUndo(node, "Create quest node");

        if (graph.entryNode == null)
        {
            graph.entryNode = node;
        }

        EditorUtility.SetDirty(node);
        EditorUtility.SetDirty(graph);
        AssetDatabase.SaveAssets();
    }
}
