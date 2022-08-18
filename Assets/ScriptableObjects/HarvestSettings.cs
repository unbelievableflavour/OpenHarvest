using System.Collections.Generic;
using UnityEngine;

public class HarvestSettings : ScriptableObject
{
    public bool isPCMode;
    public bool enableSandbox;

    [Header("Random World Generator")]

    public bool showAllModulesOnStart = false;
    public bool showOverlapColliders = false;
    public bool showRemovedOverlapColliders = false;
}