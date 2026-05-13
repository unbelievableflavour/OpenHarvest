using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeEditor(typeof(NpcChatNode))]
public class NpcChatNodeEditor : NodeEditor
{
    public override int GetWidth()
    {
        return 520;
    }

    public override void OnBodyGUI()
    {
        serializedObject.Update();

        var node = target as NpcChatNode;
        if (node == null)
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }

        NodeEditorGUILayout.PortField(node.GetInputPort("inFlow"));
        EditorGUILayout.Space(4f);

        SerializedProperty bodyProp = serializedObject.FindProperty("body");
        EditorGUILayout.PropertyField(bodyProp, includeChildren: true);

        EditorGUILayout.Space(4f);
        EditorGUILayout.LabelField("Choices", EditorStyles.boldLabel);

        SerializedProperty choicesProp = serializedObject.FindProperty("choices");
        EditorGUI.BeginChangeCheck();
        NodeEditorGUILayout.DynamicPortList(
            "choices",
            typeof(string),
            choicesProp.serializedObject,
            NodePort.IO.Output,
            Node.ConnectionType.Override,
            Node.TypeConstraint.None);
        bool didChangeChoices = EditorGUI.EndChangeCheck();

        EditorGUILayout.Space(8f);
        if (GUILayout.Button("Delete Node"))
        {
            DeleteNode(node);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        serializedObject.ApplyModifiedProperties();

        if (didChangeChoices)
        {
            node.UpdatePorts();
            EditorUtility.SetDirty(node);
        }
    }

    private static void DeleteNode(NpcChatNode node)
    {
        if (node == null)
        {
            return;
        }

        NpcChatGraph graph = node.graph as NpcChatGraph;
        if (graph == null)
        {
            return;
        }

        Undo.RecordObject(graph, "Delete chat node");
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
