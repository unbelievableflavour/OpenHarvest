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

public class HarvestSettings : ScriptableObject
{
    public bool isPCMode = false;

    [Header("Time")]
    public TimeManipulation forceTime = TimeManipulation.None;

    [Header("Developer Options")]
    public bool enableIngameConsole = false;
    public bool enableDevMode = false;

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
#endif
}
