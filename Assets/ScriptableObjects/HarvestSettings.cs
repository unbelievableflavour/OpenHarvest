using System.Collections.Generic;
using UnityEngine;

public class HarvestSettings : ScriptableObject
{
    public const string k_MyCustomSettingsPath = "Assets/Editor/MyCustomSettings.asset";

    [SerializeField]
    public bool isPCMode;

    [SerializeField]
    public bool enableSandbox;
}