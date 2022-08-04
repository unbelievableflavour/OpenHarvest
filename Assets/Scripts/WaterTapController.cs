using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTapController : MonoBehaviour
{
    public GameObject waterParticles;
    public AudioSource waterSound;

    void Start()
    {
        //CheckIfWaterShouldFlow(360);
    }

    public void CheckIfWaterShouldFlow(float change)
    {
        if(change > 200)
        {
            waterParticles.SetActive(true);
            waterSound.enabled = true;
        } else
        {
            waterParticles.SetActive(false);
            waterSound.enabled = false;
        }
    }
}
