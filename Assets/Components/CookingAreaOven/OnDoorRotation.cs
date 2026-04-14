using BNG;
using UnityEngine;
using UnityEngine.Events;

public class OnDoorRotation : MonoBehaviour
{
    public UnityEvent onDoorOpen;
    public UnityEvent onDoorClosed;
    public DoorHelper doorHelper;

    private bool openedOnce = false;
    private bool doorIsOpen = false;

    void Update()
    {
        if (!openedOnce)
        {
            if(doorHelper.angle != 0)
            {
                openedOnce = true;
            } else
            {
                return;
            }
        }

        if (doorHelper.angle == 0)
        {
            DoorClosed();
        } else
        {
            DoorOpened();
        }
    }

    private void DoorOpened()
    {
        if (doorIsOpen) {
            return;
        }

        doorIsOpen = true;
        onDoorOpen.Invoke();
    }

    private void DoorClosed()
    {
        if (!doorIsOpen)
        {
            return;
        }

        doorIsOpen = false;
        onDoorClosed.Invoke();
    }
}
