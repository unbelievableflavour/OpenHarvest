using UnityEngine;

public class DirtCollisionController : MonoBehaviour
{
    public GameObject dirtParticle;

    void OnTriggerExit(Collider other)
    {
        if (other.tag != "Shovel" && other.tag != "Hoe")
        {
            return;
        }

        var otherObjectPosition = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
        GameObject particle = Instantiate(
            dirtParticle, 
            new Vector3(
                otherObjectPosition.x, 
                transform.position.y, 
                otherObjectPosition.z
            ), 
            dirtParticle.transform.rotation
        );
        Object.Destroy(particle, 2.0f);
    }
}
