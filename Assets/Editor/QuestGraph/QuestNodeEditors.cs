using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

internal static class QuestNodeEditorDrawing
{
    internal static void EnsureSerializedObjectMatchesTarget(NodeEditor editor)
    {
        if (editor == null || editor.target == null)
        {
            return;
        }

        if (editor.serializedObject == null || editor.serializedObject.targetObject != editor.target)
        {
            editor.serializedObject = new SerializedObject(editor.target);
        }
    }

    internal static void PropertyFieldOrSkip(SerializedObject serializedObject, string propertyName)
    {
        if (serializedObject == null)
        {
            return;
        }

        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property == null)
        {
            EditorGUILayout.HelpBox(
                $"Missing serialized property '{propertyName}' on {serializedObject.targetObject?.GetType().Name}. Reimport the quest asset or restart the Unity Editor.",
                MessageType.Warning);
            return;
        }

        EditorGUILayout.PropertyField(property);
    }
}

[CustomNodeEditor(typeof(QuestChatNode))]
public class QuestChatNodeEditor : NodeEditor
{
    public override int GetWidth()
    {
        return 420;
    }

    public override void OnBodyGUI()
    {
        if (target == null)
        {
            return;
        }

        QuestNodeEditorDrawing.EnsureSerializedObjectMatchesTarget(this);
        serializedObject.Update();
        NodeEditorGUILayout.PortField((target as QuestChatNode)?.GetInputPort("inFlow"));
        NodeEditorGUILayout.PortField((target as QuestChatNode)?.GetOutputPort("next"));
        DrawDefaultNodeFields();
        DrawDeleteButton(target as QuestNodeBase);
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDefaultNodeFields()
    {
        QuestNodeEditorDrawing.PropertyFieldOrSkip(serializedObject, "targetNpc");
        QuestNodeEditorDrawing.PropertyFieldOrSkip(serializedObject, "body");
        QuestNodeEditorDrawing.PropertyFieldOrSkip(serializedObject, "tip");
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

[CustomNodeEditor(typeof(QuestFinishNode))]
public class QuestFinishNodeEditor : NodeEditor
{
    public override int GetWidth()
    {
        return 420;
    }

    public override void OnBodyGUI()
    {
        if (target == null)
        {
            return;
        }

        QuestNodeEditorDrawing.EnsureSerializedObjectMatchesTarget(this);
        serializedObject.Update();
        NodeEditorGUILayout.PortField((target as QuestFinishNode)?.GetInputPort("inFlow"));
        NodeEditorGUILayout.PortField((target as QuestFinishNode)?.GetOutputPort("next"));
        DrawDefaultNodeFields();
        DrawDeleteButton(target as QuestNodeBase);
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDefaultNodeFields()
    {
        QuestNodeEditorDrawing.PropertyFieldOrSkip(serializedObject, "targetNpc");
        QuestNodeEditorDrawing.PropertyFieldOrSkip(serializedObject, "coinReward");
        QuestNodeEditorDrawing.PropertyFieldOrSkip(serializedObject, "handRewardItem");
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
        if (target == null)
        {
            return;
        }

        QuestNodeEditorDrawing.EnsureSerializedObjectMatchesTarget(this);
        serializedObject.Update();
        NodeEditorGUILayout.PortField((target as QuestGiftNode)?.GetInputPort("inFlow"));
        NodeEditorGUILayout.PortField((target as QuestGiftNode)?.GetOutputPort("next"));
        DrawDefaultNodeFields();
        DrawDeleteButton(target as QuestNodeBase);
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDefaultNodeFields()
    {
        QuestNodeEditorDrawing.PropertyFieldOrSkip(serializedObject, "targetNpc");
        QuestNodeEditorDrawing.PropertyFieldOrSkip(serializedObject, "giftPrompt");
        QuestNodeEditorDrawing.PropertyFieldOrSkip(serializedObject, "tip");
        QuestNodeEditorDrawing.PropertyFieldOrSkip(serializedObject, "requiredItem");
        QuestNodeEditorDrawing.PropertyFieldOrSkip(serializedObject, "requiredAmount");
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

[CustomNodeEditor(typeof(QuestWorldObjectiveNode))]
public class QuestWorldObjectiveNodeEditor : NodeEditor
{
    public override int GetWidth()
    {
        return 420;
    }

    public override void OnBodyGUI()
    {
        if (target == null)
        {
            return;
        }

        QuestNodeEditorDrawing.EnsureSerializedObjectMatchesTarget(this);
        serializedObject.Update();
        NodeEditorGUILayout.PortField((target as QuestWorldObjectiveNode)?.GetInputPort("inFlow"));
        NodeEditorGUILayout.PortField((target as QuestWorldObjectiveNode)?.GetOutputPort("next"));
        DrawDefaultNodeFields();
        DrawDeleteButton(target as QuestNodeBase);
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDefaultNodeFields()
    {
        QuestNodeEditorDrawing.PropertyFieldOrSkip(serializedObject, "targetNpc");
        QuestNodeEditorDrawing.PropertyFieldOrSkip(serializedObject, "objectiveKey");
        QuestNodeEditorDrawing.PropertyFieldOrSkip(serializedObject, "tip");
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
