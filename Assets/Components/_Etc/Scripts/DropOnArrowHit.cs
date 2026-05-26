using UnityEngine;

public class DropOnArrowHit : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        var collider_tag = collision.gameObject.tag;

        if (collider_tag != "Arrow")
        {
            return;
        }

        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody)
        {
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
        }

        var transform = GetComponent<Transform>();

        if (transform)
        {
            transform.SetParent(null);
        }

        var hasSpawnManager = GetComponent<HasSpawnManager>();
        if (hasSpawnManager)
        {
            hasSpawnManager.resetSpawnManagerIfLastGrabbed();
        }

        var moveTransformToTargets = GetComponent<MoveTransformToTargets>();
        if (moveTransformToTargets)
        {
            moveTransformToTargets.StopMoving();
        }

        Destroy(collision.gameObject);
    }
}
