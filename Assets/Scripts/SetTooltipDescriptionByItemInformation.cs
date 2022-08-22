using UnityEngine;

public class SetTooltipDescriptionByItemInformation : MonoBehaviour
{
    public TooltipInformation tooltip;

    void Awake()
    {
        var item = Definitions.GetItemFromObject(this.gameObject);
        if (item == null)
        {
            return;
        }

        if (!tooltip || !tooltip.textComponent)
        {
            return;
        }

        tooltip.textComponent.text = item.name + "\n" + "\n" + item.description;
    }
}
