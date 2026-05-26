using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowParticlesWhenClose : MonoBehaviour
{
    public float maxShowDistance = 10f;
    public List<ParticleSystem> particlesToToggle;

    bool shouldBeEnabled = false;
    Transform mainCam;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main.transform;
    }

    void Update()
    {
        float currentDistance = Vector3.Distance(transform.position, mainCam.position);
        UpdateActiveState(currentDistance <= maxShowDistance);
    }

    void UpdateActiveState(bool newState)
    {
        if(shouldBeEnabled == newState)
        {
            return;
        }
        shouldBeEnabled = newState;

        foreach(ParticleSystem particleToToggle in particlesToToggle) {
            if(shouldBeEnabled) {
                particleToToggle.Play();
            } else {
                particleToToggle.Stop();
            }
        }
    }
}
