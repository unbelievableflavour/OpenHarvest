using UnityEngine;

public class SetTooltipDescriptionByItemInformation : MonoBehaviour
{
    public TooltipInformation tooltip;

    void Awake()
    {
        var itemInformation = GetComponent<ItemInformation>();
        Item itemInfo = itemInformation.getItemInfo();

        if (itemInfo == null)
        {
            return;
        }

        if (!tooltip || !tooltip.textComponent)
        {
            return;
        }

        tooltip.textComponent.text = itemInfo.name + "\n" + "\n" + itemInfo.description;
    }
}
