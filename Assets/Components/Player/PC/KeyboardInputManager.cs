using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardInputManager : MonoBehaviour
{
    [SerializeField]
    private Vector3 pcPanelLocalPosition = new Vector3(0f, -0.05f, 0.8f);

    [SerializeField]
    private Vector3 pcPanelLocalEulerAngles = new Vector3(0f, 180f, 0f);

    [SerializeField]
    private Vector3 pcPanelLocalScale = new Vector3(0.004f, 0.004f, 0.004f);

    private PlacementHandPanel pcPlacementPanel;

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
        if (pcPlacementPanel != null)
        {
            pcPlacementPanel.ConfigureVisibilityForPcToggle();
            return;
        }

        Camera targetCamera = Camera.main != null ? Camera.main : GetComponentInChildren<Camera>(true);
        Transform parent = targetCamera != null ? targetCamera.transform : transform;

        Transform existingAnchor = parent.Find("PCPlacementPanelAnchor");
        if (existingAnchor == null)
        {
            GameObject anchor = new GameObject("PCPlacementPanelAnchor");
            anchor.transform.SetParent(parent, false);
            anchor.transform.localPosition = pcPanelLocalPosition;
            anchor.transform.localRotation = Quaternion.Euler(pcPanelLocalEulerAngles);
            anchor.transform.localScale = Vector3.one;
            existingAnchor = anchor.transform;
        }
        else
        {
            existingAnchor.localPosition = pcPanelLocalPosition;
            existingAnchor.localRotation = Quaternion.Euler(pcPanelLocalEulerAngles);
            existingAnchor.localScale = Vector3.one;
        }

        pcPlacementPanel = existingAnchor.GetComponent<PlacementHandPanel>();
        if (pcPlacementPanel == null)
        {
            pcPlacementPanel = existingAnchor.gameObject.AddComponent<PlacementHandPanel>();
        }

        pcPlacementPanel.ConfigurePanelLocalTransform(Vector3.zero, Vector3.zero, pcPanelLocalScale);
        pcPlacementPanel.ConfigureVisibilityForPcToggle();
    }
}
