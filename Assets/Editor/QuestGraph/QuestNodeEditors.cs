using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeEditor(typeof(QuestChatNode))]
public class QuestChatNodeEditor : NodeEditor
{
    public override int GetWidth()
    {
        return 420;
    }

    public override void OnBodyGUI()
    {
        serializedObject.Update();
        NodeEditorGUILayout.PortField((target as QuestChatNode)?.GetInputPort("inFlow"));
        NodeEditorGUILayout.PortField((target as QuestChatNode)?.GetOutputPort("next"));
        DrawDefaultNodeFields();
        DrawDeleteButton(target as QuestNodeBase);
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDefaultNodeFields()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetNpc"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("completesQuest"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("body"));
    }

    private static void DrawDeleteButton(QuestNodeBase node)
    {
        if (node == null)
        {
            return;
        }

        EditorGUILayout.Space(6f);
        if (!GUILayout.Button("Delete Node"))
        {
            return;
        }

        QuestGraph graph = node.graph as QuestGraph;
        if (graph == null)
        {
            return;
        }

        Undo.RecordObject(graph, "Delete quest node");
        if (graph.entryNode == node)
        {
            graph.entryNode = null;
        }

        graph.RemoveNode(node);
        Undo.DestroyObjectImmediate(node);
        EditorUtility.SetDirty(graph);
        AssetDatabase.SaveAssets();
    }
}

[CustomNodeEditor(typeof(QuestGiftNode))]
public class QuestGiftNodeEditor : NodeEditor
{
    public override int GetWidth()
    {
        return 420;
    }

    public override void OnBodyGUI()
    {
        serializedObject.Update();
        NodeEditorGUILayout.PortField((target as QuestGiftNode)?.GetInputPort("inFlow"));
        NodeEditorGUILayout.PortField((target as QuestGiftNode)?.GetOutputPort("next"));
        DrawDefaultNodeFields();
        DrawDeleteButton(target as QuestNodeBase);
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDefaultNodeFields()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetNpc"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("requiredItem"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("requiredAmount"));
    }

    private static void DrawDeleteButton(QuestNodeBase node)
    {
        if (node == null)
        {
            return;
        }

        EditorGUILayout.Space(6f);
        if (!GUILayout.Button("Delete Node"))
        {
            return;
        }

        QuestGraph graph = node.graph as QuestGraph;
        if (graph == null)
        {
            return;
        }

        Undo.RecordObject(graph, "Delete quest node");
        if (graph.entryNode == node)
        {
            graph.entryNode = null;
        }

        graph.RemoveNode(node);
        Undo.DestroyObjectImmediate(node);
        EditorUtility.SetDirty(graph);
        AssetDatabase.SaveAssets();
    }
}
