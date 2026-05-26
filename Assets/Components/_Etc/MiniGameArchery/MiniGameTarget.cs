using UnityEngine;

public class MiniGameTarget : MonoBehaviour
{
    MiniGameArchery miniGameArchery;

    public void SetParent(MiniGameArchery newMiniGameArchery)
    {
        miniGameArchery = newMiniGameArchery;
    }

    public void handleDestroyTarget()
    {
        miniGameArchery.handleDestroyTarget();
    }
}
