using BNG;
using UnityEngine;

public class FishingRodController : MonoBehaviour
{
    public GameObject hookPrefab;
    public GameObject connectPosition;
    public SphereCollider hookCollider;
    private GameObject hook;
    private HarvestDataTypes.Item savedItem;
    private int savedItemStack;
    void Start()
    {
        DisableFishingRod();
    }

    public void EnableFishingRod()
    {
        hook = Instantiate(hookPrefab);
        hook.transform.position = connectPosition.transform.position;
        hook.GetComponent<LookAtTransform>().LookAt = connectPosition.transform;
        hook.GetComponent<ConfigurableJoint>().connectedBody = connectPosition.GetComponent<Rigidbody>();
        GetComponent<DrawLineBetween2Transforms>().lineEnd = hook.transform;
        GetComponent<FishingRodContractor>().ShowParts(4);

        if (savedItem != null)
        {
            var slot = hook.GetComponent<Hook>().snapZone;
            
            var hookedItem = Definitions.InstantiateItemNew(savedItem.prefab);
            var hookedItemGrabbable = hookedItem.GetComponent<Grabbable>();
            if (!hookedItemGrabbable)
            {
                return;
            }
            slot.GrabGrabbable(hookedItemGrabbable);

            var itemStack = hookedItemGrabbable.GetComponent<ItemStack>();
            if (itemStack)
            {
                itemStack.SetStackSize(savedItemStack);
            }
        }
    }

    public void DisableFishingRod()
    {
        if (!hook)
        {
            return;
        }

        var heldItem = hook.GetComponent<Hook>().snapZone.HeldItem;

        if (heldItem)
        {
            savedItem = Definitions.GetItemFromObject(heldItem);
            var itemStack = heldItem.GetComponent<ItemStack>();
            savedItemStack = itemStack ? itemStack.GetStackSize() : 1;
        }
        else
        {
            savedItem = null;
            savedItemStack = 1;
        }

        Destroy(hook);
        GetComponent<FishingRodContractor>().ShowParts(1);
    }
}
