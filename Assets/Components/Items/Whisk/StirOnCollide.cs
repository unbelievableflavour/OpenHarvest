using UnityEngine;

public class StirOnCollide : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<NeedsStirring>())
        {
            return;
        }
        other.GetComponent<NeedsStirring>().InvokeStirring();
    }
}
