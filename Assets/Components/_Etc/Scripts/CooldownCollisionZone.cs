using UnityEngine;

public class CooldownCollisionZone : MonoBehaviour
{
    public BreakableObjectController breakableObjectController;

    private void OnTriggerExit(Collider other)
    {
        var collider_tag = other.gameObject.tag;

        if (collider_tag != breakableObjectController.toolUsedForBreaking)
        {
            return;
        }

        breakableObjectController.unsetCooldown();
    }
}
