using BNG;
using UnityEngine;

public class ForceSnapZoneOffset : MonoBehaviour
{
    public Vector3 LocalPositionOffset;
    public Vector3 LocalRotationOffset;

    public void Reposition(Grabbable grabbable)
    {
        grabbable.transform.localPosition = LocalPositionOffset;
        grabbable.transform.localEulerAngles = LocalRotationOffset;
    }
}
 