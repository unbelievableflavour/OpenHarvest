using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPatch : MonoBehaviour
{
    private int patchIndexInDB = 4;
    public PlacementSystem placementSystem;

    public void OnTriggerExit(Collider other) {
        var collider_tag = other.tag;

        if (collider_tag != "Shovel") {
            return;
        }

        var otherObjectPosition = other.ClosestPointOnBounds(transform.position);

        bool placementValidity = placementSystem.CheckPlacementValidity(otherObjectPosition, patchIndexInDB);
        if (!placementValidity) {
            return;
        }

        placementSystem.PlaceStructureWithoutPreview(otherObjectPosition, patchIndexInDB);
    }
}
