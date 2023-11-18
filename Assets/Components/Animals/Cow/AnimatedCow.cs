using UnityEngine;

public class AnimatedCow : MonoBehaviour
{
    public Animator animator;
    public float movesWhenFurtherAwayThen = 5f;

    private float speed = 20 * 1000;
    private bool isMoving = false;
    private float randomNumberOfSeconds = 0f;
    private Rigidbody rigidBody;
    Vector3 pointsToMoveTowards;
    private float timer;

    bool shouldMove = true;
    Transform mainCam;

    void Start()
    {
        mainCam = Camera.main.transform;
        rigidBody = GetComponent<Rigidbody>();
        //randomNumberOfSeconds = Random.Range(10, 15);
    }

    void Update()
    {
        float currentDistance = Vector3.Distance(transform.position, mainCam.position);
        UpdateActiveState(currentDistance >= movesWhenFurtherAwayThen);

        if (!shouldMove || isMoving)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer < randomNumberOfSeconds)
        {
            return;
        }

        randomNumberOfSeconds = Random.Range(10, 15);
        timer = 0;

        animator.Play("WalkingAnimation");
        Invoke("MoveInRandomDirection", 0);
    }

    void UpdateActiveState(bool newState)
    {
        if (shouldMove == newState)
        {
            return;
        }
        shouldMove = newState;

        if (!shouldMove && isMoving)
        {
            CancelInvoke("SlowDownMovement");
            SlowDownMovement();
        }
    }

    private void SlowDownMovement()
    {
        animator.Play("IdleAnimation");
        rigidBody.drag = 10;
        isMoving = false;
    }

    private void MoveInRandomDirection()
    {
        isMoving = true;
        rigidBody.drag = 0;
        pointsToMoveTowards = Random.onUnitSphere;
        pointsToMoveTowards.y = 0.0f;

        FaceForward();
        rigidBody.AddForce(pointsToMoveTowards * speed);
        var walkForRandomNumberOfSeconds = Random.Range(4, 10);

        Invoke("SlowDownMovement", walkForRandomNumberOfSeconds);
    }

    private void FaceForward()
    {
        transform.rotation = Quaternion.LookRotation(pointsToMoveTowards);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (transform.name == "RenderOnCollide") {
            // this is just a little hack to make sure animals wont walk against the RenderOnCollide objects.
            return;
        }

        SlowDownMovement();
    }
}
