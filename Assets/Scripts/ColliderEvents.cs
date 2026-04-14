using BNG;
using UnityEngine;
using UnityEngine.Events;

public class ColliderEvents : MonoBehaviour
{
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    public string triggerName = "Grabber";

    void OnTriggerEnter(Collider other)
    {
        if (other.name != triggerName)
        {
            return;
        }

        onTriggerEnter.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name != triggerName)
        {
            return;
        }

        onTriggerExit.Invoke();
    }
}