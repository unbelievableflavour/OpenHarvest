using UnityEditor;

[CustomEditor(typeof(QuestNodeBase), true)]
public class QuestNodeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty property = serializedObject.GetIterator();
        bool enterChildren = true;
        while (property.NextVisible(enterChildren))
        {
            enterChildren = false;
            if (ShouldHide(property))
            {
                continue;
            }

            EditorGUILayout.PropertyField(property, includeChildren: true);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private static bool ShouldHide(SerializedProperty property)
    {
        if (property == null)
        {
            return true;
        }

        string name = property.name;
        if (name == "m_Script")
        {
            return false;
        }

        return name == "graph"
            || name == "position"
            || name == "ports"
            || name == "inFlow"
            || name == "next";
    }
}
