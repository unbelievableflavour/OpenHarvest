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
#endif
}
