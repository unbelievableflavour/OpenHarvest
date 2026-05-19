using UnityEngine;

public class KeyboardInputManager : MonoBehaviour
{
    public PlacementHandPanel pcPlacementPanel;

    private void Awake()
    {
        EnsurePcPlacementPanelExists();
    }

    private void EnsurePcPlacementPanelExists()
    {
        pcPlacementPanel.ConfigureVisibilityForPcToggle();
    }
}
