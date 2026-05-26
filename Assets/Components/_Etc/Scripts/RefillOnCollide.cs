using UnityEngine;

public class RefillOnCollide : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<NeedsWater>())
        {
            return;
        }
        other.GetComponent<NeedsWater>().InvokeWatering();
    }
}
