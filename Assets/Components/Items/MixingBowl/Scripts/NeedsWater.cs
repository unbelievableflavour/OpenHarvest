using UnityEngine;
using UnityEngine.Events;

public class NeedsWater : MonoBehaviour
{
    public UnityEvent onWatering;

    public void InvokeWatering()
    {
        onWatering.Invoke();
    }
}
