using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SwitchSceneOnCollision))]
public class SwitchSceneOnCollisionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SwitchSceneOnCollision component = (SwitchSceneOnCollision)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Switch Scene"))
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Not in Play Mode", "Enter Play Mode first to switch scenes.", "OK");
                return;
            }

            SceneSwitcher.Instance.SwitchToScene(component.sceneIndex, component.sceneEnterLocationName);
        }
    }
}
