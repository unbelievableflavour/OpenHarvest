using BNG;
using UnityEngine;

public class OnlyLookAtTransformOnCollision : MonoBehaviour
{
    public  Transform startPosition;
    private LookAtForShopKeeper component;
    private Transform backupPlayer;

    void Start()
    {
        component = GetComponent<LookAtForShopKeeper>();
        backupPlayer = startPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag != "HeadCollision")
        {
            return;
        }

        component.LookAt = GameState.Instance.currentPlayerPosition;
        component.isLookingAtPlayer = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag != "HeadCollision")
        {
            return;
        }
        component.isLookingAtPlayer = false;
        component.LookAt = backupPlayer;
    }
}
