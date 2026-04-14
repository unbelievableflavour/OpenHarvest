using UnityEngine;
using UnityEngine.Events;

public class NeedsStirring : MonoBehaviour
{ 
    public UnityEvent onStirring;

    public void InvokeStirring()
    {
        onStirring.Invoke();
    }    
}
