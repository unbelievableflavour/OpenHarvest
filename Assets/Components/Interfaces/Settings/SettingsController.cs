using BNG;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("Toggles")]
    public Toggle useHandsOnly;
    public Toggle useSmoothLocomotion;
    public Toggle useSmoothTurning;
    public Toggle useAssistMode;
    public UnityEngine.UI.Slider smoothTurningSensitivitySlider;
    public Text smoothTurningSensitivityLabel;

    public UnityEngine.UI.Slider playerHeightOffsetSlider;
    public Text playerHeightOffsetSliderLabel;

    public UnityEngine.UI.Slider backgroundMusicVolumeSlider;
    public Text backgroundMusicVolumeLabel;

    public Dropdown resolutionScaleDropdown;
    public Toggle useShadows;
    public Toggle useApplicationSpaceWarp;

    public void UpdateSettingsWithPrefs() {
        UpdateSettingsInCanvas(GameState.Instance.settings);
    }

    public void UpdateSettingsInCanvas(Dictionary<string,string> settings)
    {
        useHandsOnly.isOn = bool.Parse(settings["useHandsOnly"]);
        useSmoothLocomotion.isOn = bool.Parse(settings["useSmoothLocomotion"]);
        useSmoothTurning.isOn = bool.Parse(settings["useSmoothTurning"]);
        smoothTurningSensitivityLabel.text = settings["smoothTurningSensitivity"];
        smoothTurningSensitivitySlider.value = float.Parse(settings["smoothTurningSensitivity"]);
        playerHeightOffsetSlider.value = float.Parse(settings["playerHeightOffset"]);
        useAssistMode.isOn = bool.Parse(settings["useAssistMode"]);
        backgroundMusicVolumeLabel.text = settings["backgroundMusicVolume"];
        backgroundMusicVolumeSlider.value = float.Parse(settings["backgroundMusicVolume"]);
        resolutionScaleDropdown.value = int.Parse(settings["resolutionScale"]);
        useShadows.isOn = bool.Parse(settings["useShadows"]);
        useApplicationSpaceWarp.isOn = bool.Parse(settings["useApplicationSpaceWarp"]);
    }

    public void UpdateHandSettings()
    {
        GameState.Instance.settings["useHandsOnly"] = useHandsOnly.isOn.ToString();
        PlayerCustomSettings.Instance.RefreshSettings();
    }

    public void UpdateLocomotionSettings()
    {
        GameState.Instance.settings["useSmoothLocomotion"] = useSmoothLocomotion.isOn.ToString();
        PlayerCustomSettings.Instance.RefreshSettings();
    }

    public void UpdateSmoothRotationSettings()
    {
        GameState.Instance.settings["useSmoothTurning"] = useSmoothTurning.isOn.ToString();
        PlayerCustomSettings.Instance.RefreshSettings();

        smoothTurningSensitivitySlider.interactable = useSmoothTurning.isOn;
    }

    public void UpdateSmoothRotationSensitivitySettings()
    {
        GameState.Instance.settings["smoothTurningSensitivity"] = smoothTurningSensitivitySlider.value.ToString();
        PlayerCustomSettings.Instance.RefreshSettings();

        smoothTurningSensitivityLabel.text = smoothTurningSensitivitySlider.value.ToString();        
    }

    public void UpdateBackgroundMusicVolumeSettings()
    {
        GameState.Instance.settings["backgroundMusicVolume"] = backgroundMusicVolumeSlider.value.ToString();
        PlayerCustomSettings.Instance.RefreshSettings();

        backgroundMusicVolumeLabel.text = (backgroundMusicVolumeSlider.value / 10).ToString();
    }

    public void UpdatePlayerHeightOffsetSlider()
    {
        GameState.Instance.settings["playerHeightOffset"] = playerHeightOffsetSlider.value.ToString();
        PlayerCustomSettings.Instance.RefreshSettings();

        playerHeightOffsetSliderLabel.text = (playerHeightOffsetSlider.value / 10).ToString();
    }

    public void UpdateUseAssistMode()
    {
        GameState.Instance.settings["useAssistMode"] = useAssistMode.isOn.ToString();
        PlayerCustomSettings.Instance.RefreshSettings();
    }

    public void UpdateSkinColor(string color)
    {
        GameState.Instance.settings["skinColor"] = color;
        PlayerCustomSettings.Instance.RefreshSettings();
    }

    public void UpdateResolutionScale()
    {
        GameState.Instance.settings["resolutionScale"] = resolutionScaleDropdown.value.ToString();
        PlayerCustomSettings.Instance.RefreshSettings();
    }

    public void UpdateShadowSettings()
    {
        GameState.Instance.settings["useShadows"] = useShadows.isOn.ToString();
        PlayerCustomSettings.Instance.RefreshSettings();
    }

    public void UpdateApplicationSpaceWarp()
    {
        GameState.Instance.settings["useApplicationSpaceWarp"] = useApplicationSpaceWarp.isOn.ToString();
        PlayerCustomSettings.Instance.RefreshSettings();
    }
}
