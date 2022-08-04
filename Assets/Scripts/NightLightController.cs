using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightLightController : MonoBehaviour
{
    public Material disabledLightMaterial;
    public Material enabledLightMaterial;

    public void DisableLights()
    {
        foreach (Transform child in transform)
        {

            var nightLight = child.GetComponent<NightLight>();

            if (!nightLight)
            {
                return;
            }

            nightLight.lanternLightMesh.material = disabledLightMaterial;

            if (nightLight.light)
            {
                nightLight.light.enabled = false;
            }
        }
    }

    public void EnableLights()
    {
        foreach (Transform child in transform)
        {

            var nightLight = child.GetComponent<NightLight>();

            if (!nightLight)
            {
                return;
            }

            nightLight.lanternLightMesh.material = enabledLightMaterial;
            if (nightLight.light)
            {
                nightLight.light.enabled = true;
            }
        }
    }
}
