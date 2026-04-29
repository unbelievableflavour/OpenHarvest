using UnityEngine;

public class UIEventSender : MonoBehaviour
{
    public void SendEvent(string eventName)
    {
        if (string.IsNullOrWhiteSpace(eventName))
        {
            return;
        }

        EventManager.Emit(eventName);
    }
}
