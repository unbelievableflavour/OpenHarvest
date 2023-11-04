using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineToTransform : MonoBehaviour {
    public LineRenderer line;

    [Header("Filled during play")]
    public Transform ConnectTo;

    void LateUpdate() {
        UpdateLine();
    }

    public void UpdateLine() {
        if(!ConnectTo){
            return;
        }

        if(line != null) {
            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, transform.InverseTransformPoint(ConnectTo.position));
        }
    }

    void OnDrawGizmos() {
        UpdateLine();
    }
}
