using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum TimeManipulation
{
    None,
    Night,
    Day,
};

public enum PlayerMode
{
    VR,
    FPS,
    Showcase,
};

public class HarvestSettings : ScriptableObject
{
    public PlayerMode playerMode = PlayerMode.VR;

    [Header("Time")]
    public TimeManipulation forceTime = TimeManipulation.None;

    [Header("Developer Options")]
    public bool enableIngameConsole = false;
    public bool enableDevMode = false;

    [Header("Start Options")]
    [Tooltip("If enabled, new games start with a large amount of money.")]
    public bool startWithLotsOfMoney = false;

    [Tooltip("If enabled, new games start with many owned placeable objects.")]
    public bool startWithLotsOfPlaceables = false;

    [Header("Random World Generator")]
    public bool showAllModulesOnStart = false;
    public bool showOverlapColliders = false;
    public bool showRemovedOverlapColliders = false;

#if UNITY_EDITOR
    [MenuItem("Harvest VR/Settings")]
    private static void OpenSettingsSO() {
        var texturePath = AssetDatabase.LoadMainAssetAtPath($"Assets/ScriptableObjects/HarvestSettings.asset");
        AssetDatabase.OpenAsset(texturePath);
    }

    public void ResetToProductionValues()
    {
        playerMode = PlayerMode.VR;
        forceTime = TimeManipulation.None;
        enableIngameConsole = false;
        enableDevMode = false;
        startWithLotsOfMoney = false;
        startWithLotsOfPlaceables = false;
        showAllModulesOnStart = false;
        showOverlapColliders = false;
        showRemovedOverlapColliders = false;
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(HarvestSettings))]
public class HarvestSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        var settings = (HarvestSettings)target;
        GUI.backgroundColor = new Color(1.0f, 0.6f, 0.6f);
        if (GUILayout.Button("Reset to production values", GUILayout.Height(28)))
        {
            if (EditorUtility.DisplayDialog(
                "Reset to production values?",
                "This will disable dev mode, the ingame console, all start-with-bonus flags and all world-generator debug toggles, and set player mode back to VR and time to None.\n\nContinue?",
                "Reset",
                "Cancel"))
            {
                Undo.RecordObject(settings, "Reset HarvestSettings to production values");
                settings.ResetToProductionValues();
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
            }
        }
        GUI.backgroundColor = Color.white;
    }
}
#endif
