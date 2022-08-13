using System.Collections.Generic;
using UnityEngine;

public class HarvestSettings : ScriptableObject
{
    public const string k_MyCustomSettingsPath = "Assets/Editor/MyCustomSettings.asset";

    public bool isPCMode;
    public bool enableSandbox;

    [Header("Random World Generator")]

    public bool showAllModulesOnStart = false;
    public bool showOverlapColliders = false;
    public bool showRemovedOverlapColliders = false;
}