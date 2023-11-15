using BNG;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

    public void RefreshSettings()
    {
        handModelSelector.ChangeHandsModel(bool.Parse(GameState.Instance.settings["useHandsOnly"]) ? 9 : 8, true);
        locomotionManager.ChangeLocomotion(bool.Parse(GameState.Instance.settings["useSmoothLocomotion"]) ? LocomotionType.SmoothLocomotion : LocomotionType.Teleport, true);
        playerRotation.RotationType = bool.Parse(GameState.Instance.settings["useSmoothTurning"]) ? RotationMechanic.Smooth : RotationMechanic.Snap; ;
        playerRotation.SmoothTurnSpeed = float.Parse(GameState.Instance.settings["smoothTurningSensitivity"]) * 20;
        BNGPlayerOffset.CharacterControllerYOffset = float.Parse(GameState.Instance.settings["playerHeightOffset"]) / 10;
        SetUseAssistMode();
        AudioManager.Instance.ChangeMusicVolume(float.Parse(GameState.Instance.settings["backgroundMusicVolume"]) / 10);
        playerColours.Refresh();

        var refreshRate = getRefreshRate();
        if (Unity.XR.Oculus.Performance.TryGetDisplayRefreshRate(out var currentRefreshRate)) {
            if (currentRefreshRate != refreshRate) {
                Unity.XR.Oculus.Performance.TrySetDisplayRefreshRate(refreshRate);
            }
        }

        var pipeline = ((UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset);
        var resolutionScale = getResolutionScale();
        if (pipeline.renderScale != resolutionScale) {       
            pipeline.renderScale = resolutionScale;
        }

        bool useShadows = bool.Parse(GameState.Instance.settings["useShadows"]);
        if (isShadowsActive(pipeline) != useShadows)
        {
            pipeline.shadowDistance = useShadows ? defaultShadowDistanceValue : 0;
        }

        bool useFog = bool.Parse(GameState.Instance.settings["useFog"]);
        if (RenderSettings.fog != useFog)
        {
            RenderSettings.fog = useFog;
        }

        OVRManager.SetSpaceWarp(bool.Parse(GameState.Instance.settings["useApplicationSpaceWarp"]));
    }

    public bool isShadowsActive(UniversalRenderPipelineAsset pipeline)
    {
        return pipeline.shadowDistance != 0;
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
            return 1f;
        }

        if (GameState.Instance.settings["resolutionScale"] == "1")
        {
            return 1.25f;
        }

        if (GameState.Instance.settings["resolutionScale"] == "2")
        {
            return 1.4f;
        }

        if (GameState.Instance.settings["resolutionScale"] == "3")
        {
            return 1.5f;
        }

        return 1f;
    }
}
