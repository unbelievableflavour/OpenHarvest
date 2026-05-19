using UnityEngine;

public class WheatHeightController : MonoBehaviour
{
    private float speed = 0.1f;
    public Transform targetToMove;
    public Transform full;
    public Transform empty;

    private Transform targetDestination;

    private bool isStopped = true;

    void Update()
    {
        if (isStopped)
        {
            return;
        }

        float step = speed * Time.deltaTime;
        targetToMove.position = Vector3.MoveTowards(targetToMove.position, targetDestination.position, step);

        if (Vector3.Distance(targetToMove.position, targetDestination.position) < 0.001f)
        {
            isStopped = true;
        }
    }

    public void TransitionToFull()
    {
        targetDestination = full;
        isStopped = false;
    }

    public void TransitionToEmpty()
    {
        targetDestination = empty;
        isStopped = false;
    }
}
