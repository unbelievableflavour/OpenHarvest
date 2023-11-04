using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class ItemInspector : MonoBehaviour {
    public Vector3 TipOffset = new Vector3(1.5f, 0.2f, 0);
    public bool UseWorldYAxis = true;
    public float MaxViewDistance = 10f;

    public LineToTransform lineToTransform;
    
    [Header("Filled during play")]
    public Transform DrawLineTo;

    /// <summary>
    /// Hide this if farther than MaxViewDistance
    /// </summary>
    Transform childTransform;
    Transform lookAt;
    
    private bool shouldUpdate = false;
    private Transform originalParent;

    void Start() {
        originalParent = transform.parent;
        lookAt = Camera.main.transform;
        childTransform = transform.GetChild(0);
    }

    public void Init(Transform transform) {
        DrawLineTo = transform;
        lineToTransform.ConnectTo = transform;
    }

    public void StartUpdating() {
        shouldUpdate = true;
        transform.SetParent(DrawLineTo, false);
    }

    public void StopUpdating() {
        shouldUpdate = false;
        transform.SetParent(originalParent, false);
    }

    void Update() {
        if(!shouldUpdate) {
            return;
        }

        UpdateTooltipPosition();
    }

    public virtual void UpdateTooltipPosition() {
        if (lookAt) {
            transform.LookAt(Camera.main.transform);
        }
        else if (Camera.main != null) {
            lookAt = Camera.main.transform;
        }
        else if (Camera.main == null) {
            return;
        }

        transform.localPosition = TipOffset;

        if (UseWorldYAxis) {
            transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
            transform.position += new Vector3(0, TipOffset.y, 0);
        }

        if (childTransform) {
            childTransform.gameObject.SetActive(Vector3.Distance(transform.position, Camera.main.transform.position) <= MaxViewDistance);
        }
    }
}
