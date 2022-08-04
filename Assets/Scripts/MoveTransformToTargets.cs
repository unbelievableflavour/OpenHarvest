using UnityEngine;

public class MoveTransformToTargets : MonoBehaviour
{
    public Transform[] target;
    public float speed;
    public Animator animation;

    private int current;
    private bool isStopped = false;

    void Update()
    {
        if (isStopped)
        {
            return;
        }

        if (transform.position != target[current].position) {
            transform.position = Vector3.MoveTowards(transform.position, target[current].position, speed * Time.deltaTime);
            return;
        }

        current++;

        if (current == target.Length)
        {
            current = 0;
        }

        transform.LookAt(target[current]);
    }

    public void StopMoving()
    {
        if (animation)
        {
            animation.enabled = false;
        }
        isStopped = true;
    }
}
    
    

