using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevationChecker : MonoBehaviour
{
    [Header("Component should be disabled in dynamic areas since map height may be vary")]
    public float MinElevation = -100f;
    public float MaxElevation = 6000f;
    BNGPlayerController characterController;
    void Start()
    {
        characterController = GetComponentInChildren<BNGPlayerController>();
    }

    void FixedUpdate()
    {
        CheckPlayerElevationRespawn();
    }

    public virtual void CheckPlayerElevationRespawn()
    {
        // No need for elevation checks
        if (MinElevation == 0 && MaxElevation == 0)
        {
            return;
        }

        // Check Elevation based on Character Controller height
        if (characterController != null && (characterController.transform.position.y < MinElevation || characterController.transform.position.y > MaxElevation))
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        if (SceneSwitcher.Instance == null)
        {
            return;
        }

        Debug.Log("Player out of bounds; Returning to initial position.");
        SceneSwitcher.Instance.Respawn();
    }
}
