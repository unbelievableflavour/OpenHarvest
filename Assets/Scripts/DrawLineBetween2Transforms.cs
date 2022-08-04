using UnityEngine;

public class DrawLineBetween2Transforms : MonoBehaviour
{
    public Transform lineStart;
    public Transform lineEnd;
    LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if(!lineEnd || !lineStart)
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, lineStart.position);
        lineRenderer.SetPosition(1, lineEnd.position);
    }
}
