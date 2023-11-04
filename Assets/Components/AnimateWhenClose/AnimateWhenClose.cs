using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateWhenClose : MonoBehaviour
{
    public List<Animator> animators = new List<Animator>();
    public int maxShowDistance = 10;

    Transform mainCam;

    bool isEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main.transform;
        DisableAnimators();
    }

    // Update is called once per frame
    void Update()
    {
        // Show if within range
        float currentDistance = Vector3.Distance(transform.position, mainCam.position);
        if(currentDistance > maxShowDistance) {
            DisableAnimators();
            return;
        }

        EnableAnimators();
    }

    void DisableAnimators() {
        if(!isEnabled) {
            return;
        }

        isEnabled = false;
        foreach(Animator animator in animators) {
            animator.enabled = false;
        }
    }

    void EnableAnimators() {
        if(isEnabled) {
            return;
        }

        isEnabled = true;
        foreach(Animator animator in animators) {
            animator.enabled = true;
        }
    }
}
