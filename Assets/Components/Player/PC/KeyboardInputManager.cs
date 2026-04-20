using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardInputManager : MonoBehaviour
{
    public PlacementHandPanel pcPlacementPanel;

    private void Awake()
    {
        EnsurePcPlacementPanelExists();
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            if (GameState.Instance.GetMode() == "default")
            {
                GameState.Instance.SwitchToMode("build");
            }
            else
            {
                GameState.Instance.SwitchToMode("default");
            }
        }
    }

    private void EnsurePcPlacementPanelExists()
    {
        pcPlacementPanel.ConfigureVisibilityForPcToggle();
    }
}
