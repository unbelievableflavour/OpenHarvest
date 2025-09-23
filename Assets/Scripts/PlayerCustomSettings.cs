using BNG;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Add this back

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
        Application.targetFrameRate = (int)refreshRate;
        Debug.Log($"Set refresh rate to {refreshRate}Hz");

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
