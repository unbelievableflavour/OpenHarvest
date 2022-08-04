using UnityEngine;

public class AnimatedChicken : MonoBehaviour
{
    public Animator animator;

    private float speed = 20 * 1000;
    private bool isMoving = false;
    private float randomNumberOfSeconds = 0f;
    private Rigidbody rigidBody;
    Vector3 pointsToMoveTowards;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        //randomNumberOfSeconds = Random.Range(10, 15);
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
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
        var walkForRandomNumberOfSeconds = Random.Range(3, 5);

        Invoke("SlowDownMovement", walkForRandomNumberOfSeconds);
    }

    private void FaceForward()
    {
        transform.rotation = Quaternion.LookRotation(pointsToMoveTowards);
    }
}
