using UnityEngine;

public class AnimatedFish : MonoBehaviour
{
    public Animator fishAnimator;

    private float speed = 100;
    private bool isMoving = false;
    private float randomNumberOfSeconds = 0f;
    private Rigidbody rigidBody;
    Vector3 pointsToMoveTowards;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            SlowDownFish();
            return;
        }

        timer += Time.deltaTime;
        if (timer < randomNumberOfSeconds)
        {
            return;
        }

        randomNumberOfSeconds = Random.Range(3, 6);
        timer = 0;
        SwimInRandomDirection();
        fishAnimator.Play("Restart");
    }

    private void SlowDownFish()
    {
        if (rigidBody.linearVelocity.magnitude < 0.2)
        {
            isMoving = false;
            return;
        }

        var currentVelocity = GetComponent<Rigidbody>().linearVelocity;
        var oppositeForce = -currentVelocity;
        GetComponent<Rigidbody>().AddForce(oppositeForce.x, oppositeForce.y, oppositeForce.z);
    }

    private void SwimInRandomDirection()
    {
        isMoving = true;
        pointsToMoveTowards = Random.onUnitSphere;
        pointsToMoveTowards.y = 0.0f;

        FaceForward();
        rigidBody.AddForce(pointsToMoveTowards * speed);
    }

    private void FaceForward()
    {
        transform.rotation = Quaternion.LookRotation(pointsToMoveTowards);
    }
}
