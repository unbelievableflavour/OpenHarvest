using System.Collections.Generic;
using UnityEngine;

public class HarvestSettings : ScriptableObject
{
    public bool isPCMode = false;

    [Header("Developer Options")]

    public bool enableIngameConsole = false;
    public bool enableDevMode = false;

    [Header("Random World Generator")]

    public bool showAllModulesOnStart = false;
    public bool showOverlapColliders = false;
    public bool showRemovedOverlapColliders = false;
}
