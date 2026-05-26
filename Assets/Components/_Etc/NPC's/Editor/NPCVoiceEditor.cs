using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NPCVoice))]
public class NPCVoiceEditor : Editor
{
    private const string TestLine = "The quick brown fox jumps over the lazy dog.";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(!Application.isPlaying))
        {
            if (GUILayout.Button("Speak test line"))
            {
                ((NPCVoice)target).Speak(TestLine);
            }

            if (GUILayout.Button("Stop"))
            {
                ((NPCVoice)target).StopSpeaking();
            }
        }

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter play mode to preview the voice.", MessageType.Info);
        }
    }
}
