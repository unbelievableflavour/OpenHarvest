using System.Collections.Generic;
using UnityEngine;

public class Sprinkler : MonoBehaviour
{
    public GameObject sprinklerParticles;
    public RotateObject sprinklerRotator;
    public List<SoilBehaviour> patchesToWater;

    public void EnableSprinklers()
    {
        CancelInvoke("SetDisabled");
        SetEnabled();
        Invoke("SetDisabled", 15.0f);

        foreach (var patch in patchesToWater)
        {
            patch.setWatered(true);
            patch.plantManager.Water();
        }
    }

    void SetEnabled()
    {
        sprinklerRotator.enabled = true;
        sprinklerParticles.SetActive(true);
    }

    void SetDisabled()
    {
        sprinklerRotator.enabled = false;
        sprinklerParticles.SetActive(false);
    }
}
