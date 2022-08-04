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
        handModelSelector.ChangeHandsModel(bool.Parse(GameState.settings["useHandsOnly"]) ? 9 : 8, true);
        locomotionManager.ChangeLocomotion(bool.Parse(GameState.settings["useSmoothLocomotion"]) ? LocomotionType.SmoothLocomotion : LocomotionType.Teleport, true);
        playerRotation.RotationType = bool.Parse(GameState.settings["useSmoothTurning"]) ? RotationMechanic.Smooth : RotationMechanic.Snap; ;
        playerRotation.SmoothTurnSpeed = float.Parse(GameState.settings["smoothTurningSensitivity"]) * 20;
        BNGPlayerOffset.CharacterControllerYOffset = float.Parse(GameState.settings["playerHeightOffset"]) / 10;
        SetUseAssistMode();
        AudioManager.Instance.ChangeMusicVolume(float.Parse(GameState.settings["backgroundMusicVolume"]) / 10);
        playerColours.Refresh();

        var pipeline = ((UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset);
        if (pipeline.renderScale != getResolutionScale()) {       
            pipeline.renderScale = getResolutionScale();
        }

        if (isShadowsActive(pipeline) != bool.Parse(GameState.settings["useShadows"]))
        {
            pipeline.shadowDistance = bool.Parse(GameState.settings["useShadows"]) ? defaultShadowDistanceValue : 0;
        }

        OVRManager.SetSpaceWarp(bool.Parse(GameState.settings["useApplicationSpaceWarp"]));
    }

    public bool isShadowsActive(UniversalRenderPipelineAsset pipeline)
    {
        return pipeline.shadowDistance != 0;
    }

    public float getResolutionScale()
    {
        if(GameState.settings["resolutionScale"] == "0")
        {
            return 1f;
        }

        if (GameState.settings["resolutionScale"] == "1")
        {
            return 1.25f;
        }

        if (GameState.settings["resolutionScale"] == "2")
        {
            return 1.4f;
        }

        if (GameState.settings["resolutionScale"] == "3")
        {
            return 1.5f;
        }

        return 1f;
    }
}
