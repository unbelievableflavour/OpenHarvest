using BNG;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StackableSnapZone : MonoBehaviour
{
    public List<ControllerBinding> GrabSingleInput = new List<ControllerBinding>() { ControllerBinding.None };
    public Text stackNumber;
    private SnapZone snapZone;

    /// <summary>
    /// Optional Unity Event to be called when stack is updated.
    /// </summary>
    public UnityEvent OnStackUpdateEvent;

    void Start()
    {
        snapZone = GetComponent<SnapZone>();
    }

    public void AttachItem(Grabbable item)
    {
        if (itemIsStackable(item) == false)
        {
            return;
        }

        snapZone.HeldItem.GetComponent<ItemStack>().IncreaseStack(item.GetComponent<ItemStack>().GetStackSize());
        snapZone.HeldItem.GetComponent<ItemStack>().stackindicator.gameObject.SetActive(false);
        updateStackText(snapZone.HeldItem.GetComponent<ItemStack>().GetStackSize());    
        Destroy(item.gameObject);
        OnStackUpdateEvent.Invoke();
    }

    public bool itemIsStackable(Grabbable itemGrabbable)
    {
        //if snapzone has never been active yet
        if(snapZone == null)
        {
            snapZone = GetComponent<SnapZone>();
        }

        var itemStack = itemGrabbable.GetComponent<ItemStack>();
        if (!itemStack)
        {
            return false;
        }

        var itemInSnapZone = Definitions.GetItemFromObject(snapZone.HeldItem);
        var item = Definitions.GetItemFromObject(itemGrabbable);

        if (itemInSnapZone.itemId != item.itemId)
        {
            return false;
        }
        return true;
    }

    public void DetachItem(Grabbable item)
    {
        var itemStack = item.GetComponent<ItemStack>();
        if (!itemStack)
        {
            updateStackText(0);
            snapZone.HeldItem = null;
            return;
        }

        if (itemStack.GetStackSize() <= 1) {
            updateStackText(0);
            snapZone.HeldItem = null;
            return;
        }

        if(GrabSingleKeyDown())
        {
            var currentItemStackSize = itemStack.GetStackSize() - 1;
            itemStack.SetStackSize(1);

            var itemInfo = Definitions.GetItemFromObject(item);
            var newItem = Definitions.InstantiateItemNew(itemInfo.prefab);
            newItem.GetComponent<ItemStack>().SetStackSize(currentItemStackSize);
            newItem.GetComponent<ItemStack>().stackindicator.gameObject.SetActive(false);

            snapZone.HeldItem = null;
            snapZone.GrabGrabbable(newItem.GetComponent<Grabbable>());
            updateStackText(newItem.GetComponent<ItemStack>().GetStackSize());
            OnStackUpdateEvent.Invoke();
            return;
        }

        updateStackText(0);
        snapZone.HeldItem = null;
    }

    public void updateStackText(int stackSize)
    {
        if(stackSize <= 1)
        {
            stackNumber.gameObject.SetActive(false);
            return;
        }

        stackNumber.text = stackSize.ToString();
        stackNumber.gameObject.SetActive(true);
    }

    public virtual bool GrabSingleKeyDown()
    {
        // Check for bound controller button
        for (int x = 0; x < GrabSingleInput.Count; x++)
        {
            if (InputBridge.Instance.GetControllerBindingValue(GrabSingleInput[x]))
            {
                return true;
            }
        }

        return false;
    }

    public void UnsnapItem(Grabbable item)
    {
        if (itemIsStackable(item) == false)
        {
            return;
        }

        if(snapZone.HeldItem.GetComponent<ItemStack>().GetStackSize() > 1)
        {
            snapZone.HeldItem.GetComponent<ItemStack>().stackindicator.gameObject.SetActive(true);
        }
        
        updateStackText(snapZone.HeldItem.GetComponent<ItemStack>().GetStackSize());
    }

    public void SnapItem(Grabbable item)
    {
        if (itemIsStackable(item) == false)
        {
            return;
        }

        snapZone.HeldItem.GetComponent<ItemStack>().stackindicator.gameObject.SetActive(false);
        updateStackText(item.GetComponent<ItemStack>().GetStackSize());
    }
}
