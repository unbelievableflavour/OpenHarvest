using BNG;
using UnityEngine;
using UnityEngine.UI;

public class StackHelp : MonoBehaviour
{
    public SnapZone snapZone;
    Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

    public void ShowText()
    {
        if (!text)
        {
            return;
        }

        if (!snapZone.HeldItem)
        {
            return;
        }

        if (!snapZone.HeldItem.GetComponent<ItemStack>())
        {
            return;
        }

        if (snapZone.HeldItem.GetComponent<ItemStack>().GetStackSize() < 2)
        {
            return;
        }

        text.enabled = true;
    }

    public void HideText()
    {
        text.enabled = false;
    }
}
