using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

[DefaultExecutionOrder(-1000)]
public class HardwareController : MonoBehaviour
{
    public HarvestSettings HarvestSettings;
    public GameObject VRPlayerObject;
    public GameObject PCPlayerObject;

    void Awake()
    {
        if (HarvestSettings.playerMode == PlayerMode.VR)
        {
            Instantiate(VRPlayerObject);
            StartCoroutine(ForceUiInputRebindNextFrame());
            return;
        }

        if (HarvestSettings.playerMode == PlayerMode.FPS)
        {
            Instantiate(PCPlayerObject);
            return;
        }
    }

    private static IEnumerator ForceUiInputRebindNextFrame()
    {
        // Let newly spawned rig components initialize before re-binding UI input modules.
        yield return null;

        EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        for (int i = 0; i < eventSystems.Length; i++)
        {
            EventSystem eventSystem = eventSystems[i];
            if (eventSystem == null || !eventSystem.isActiveAndEnabled)
            {
                continue;
            }

            InputSystemUIInputModule inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputModule != null && inputModule.isActiveAndEnabled)
            {
                inputModule.enabled = false;
                inputModule.enabled = true;
            }

            eventSystem.UpdateModules();
            BaseInputModule currentModule = eventSystem.currentInputModule;
            if (currentModule != null)
            {
                currentModule.DeactivateModule();
                currentModule.ActivateModule();
            }
        }
    }
}
