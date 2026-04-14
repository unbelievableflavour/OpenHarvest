using BNG;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Add this back
using UnityEngine.XR;
using UnityEngine.XR.Management; // Add this
using UnityEngine.XR.OpenXR; // Add this
using UnityEngine.XR.OpenXR.Features.Meta; // Add this for Meta extension methods
using Unity.Collections; // Add this for Allocator

public class PlayerCustomSettings : MonoBehaviour
{
	// Singleton instance.
	public static PlayerCustomSettings Instance = null;

	public BNGPlayerController BNGPlayerOffset;
    public LocomotionManager locomotionManager;
    public HandModelSelector handModelSelector;
    public PlayerRotationCustom playerRotation;
    public PlayerColours playerColours;

    public event EventHandler assistModeToggled;

    private int defaultShadowDistanceValue = 10;

    private UniversalRenderPipelineAsset clonedPipeline;
    private bool pipelineCloned = false;

    // Initialize the instance.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        RefreshSettings();
    }

    public void SetUseAssistMode()
    {
        assistModeToggled?.Invoke(this, null);
    }

    private void EnsureClonedPipeline()
    {
        if (!pipelineCloned)
        {
            var originalPipeline = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
            if (originalPipeline != null)
            {
                clonedPipeline = Instantiate(originalPipeline);
                QualitySettings.renderPipeline = clonedPipeline;
                GraphicsSettings.defaultRenderPipeline = clonedPipeline;
                pipelineCloned = true;
                Debug.Log("Created cloned URP pipeline for runtime modifications");
            }
        }
    }

    private void SetDisplayRefreshRate(float targetRefreshRate)
    {
        try
        {
            Debug.Log($"Setting refresh rate to {targetRefreshRate}Hz");
            
            // Method 1: Use Application.targetFrameRate (should work with OpenXR)
            Application.targetFrameRate = (int)targetRefreshRate;
            QualitySettings.vSyncCount = 0;
            
            Debug.Log($"Set Application.targetFrameRate to {targetRefreshRate}Hz");
            
            // Method 2: Try the OpenXR display subsystem approach if available
            var displaySubsystem = XRGeneralSettings.Instance
                ?.Manager
                ?.activeLoader
                ?.GetLoadedSubsystem<XRDisplaySubsystem>();

            if (displaySubsystem != null && displaySubsystem.running)
            {
                // Get the supported refresh rates.
                // If you will save the refresh rate values for longer than this frame, pass
                // Allocator.Persistent and remember to Dispose the array when you are done with it.
                if (displaySubsystem.TryGetSupportedDisplayRefreshRates(
                        Allocator.Temp,
                        out var refreshRates))
                {
                    Debug.Log($"Found {refreshRates.Length} supported refresh rates");
                    
                    // Find the closest supported refresh rate to our target
                    float closestRate = FindClosestRefreshRate(refreshRates, targetRefreshRate);
                    
                    // Request the closest refresh rate
                    // Returns false if you request a value that is not in the refreshRates array.
                    bool success = displaySubsystem.TryRequestDisplayRefreshRate(closestRate);
                    
                    if (success)
                    {
                        Debug.Log($"Successfully requested refresh rate: {closestRate}Hz");
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to request refresh rate: {closestRate}Hz");
                    }
                    
                    // Dispose the native array
                    refreshRates.Dispose();
                }
                else
                {
                    Debug.LogWarning("Failed to get supported display refresh rates");
                }
            }
            else
            {
                Debug.LogWarning("XR Display subsystem not available or not running");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error setting refresh rate: {e.Message}");
        }
    }

    private float FindClosestRefreshRate(Unity.Collections.NativeArray<float> supportedRates, float targetRate)
    {
        if (supportedRates.Length == 0)
            return targetRate;

        float closestRate = supportedRates[0];
        float closestDifference = Mathf.Abs(supportedRates[0] - targetRate);

        for (int i = 1; i < supportedRates.Length; i++)
        {
            float difference = Mathf.Abs(supportedRates[i] - targetRate);
            if (difference < closestDifference)
            {
                closestRate = supportedRates[i];
                closestDifference = difference;
            }
        }

        return closestRate;
    }

    public void RefreshSettings()
    {
        handModelSelector.ChangeHandsModel(bool.Parse(GameState.Instance.settings["useHandsOnly"]) ? 9 : 8, true);
        locomotionManager.ChangeLocomotion(bool.Parse(GameState.Instance.settings["useSmoothLocomotion"]) ? LocomotionType.SmoothLocomotion : LocomotionType.Teleport, true);
        playerRotation.RotationType = bool.Parse(GameState.Instance.settings["useSmoothTurning"]) ? RotationMechanic.Smooth : RotationMechanic.Snap;
        playerRotation.SmoothTurnSpeed = float.Parse(GameState.Instance.settings["smoothTurningSensitivity"]) * 20;
        BNGPlayerOffset.CharacterControllerYOffset = float.Parse(GameState.Instance.settings["playerHeightOffset"]) / 10;
        SetUseAssistMode();
        AudioManager.Instance.ChangeMusicVolume(float.Parse(GameState.Instance.settings["backgroundMusicVolume"]) / 10);
        playerColours.Refresh();

        // Ensure we have a cloned pipeline for modifications
        EnsureClonedPipeline();

        var refreshRate = getRefreshRate();
        SetDisplayRefreshRate(refreshRate); // Use the new function

        var resolutionScale = getResolutionScale();
        clonedPipeline.renderScale = resolutionScale;

        bool useShadows = bool.Parse(GameState.Instance.settings["useShadows"]);
        clonedPipeline.shadowDistance = useShadows ? defaultShadowDistanceValue : 0;

        bool useFog = bool.Parse(GameState.Instance.settings["useFog"]);
        if (RenderSettings.fog != useFog)
        {
            RenderSettings.fog = useFog;
        }
    }

    public float getRefreshRate()
    {
        if (GameState.Instance.settings["refreshRate"] == "1")
        {
            return 80f;
        }

        if (GameState.Instance.settings["refreshRate"] == "2")
        {
            return 90f;
        }

        if (GameState.Instance.settings["refreshRate"] == "3")
        {
            return 120f;
        }

        return 72f;
    }

    public float getResolutionScale()
    {
        if(GameState.Instance.settings["resolutionScale"] == "0")
        {
            return 0.5f;
        }

        if(GameState.Instance.settings["resolutionScale"] == "1")
        {
            return 0.75f;
        }

        if(GameState.Instance.settings["resolutionScale"] == "2")
        {
            return 1f;
        }

        if (GameState.Instance.settings["resolutionScale"] == "3")
        {
            return 1.25f;
        }

        if (GameState.Instance.settings["resolutionScale"] == "5")
        {
            return 1.5f;
        }

        if (GameState.Instance.settings["resolutionScale"] == "6")
        {
            return 1.75f;
        }

        if (GameState.Instance.settings["resolutionScale"] == "7")
        {
            return 2f;
        }

        return 1f;
    }
}
