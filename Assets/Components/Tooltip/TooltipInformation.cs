using UnityEngine;
using UnityEngine.UI;

public class TooltipInformation : MonoBehaviour
{
    public Text textComponent;

    void Start()
    {
        var itemInformation = transform.parent.GetComponent<ItemInformation>();
        Item itemInfo = itemInformation.getItemInfo();

        if (itemInfo == null)
        {
            return;
        }

        if (!textComponent)
        {
            return;
        }

        textComponent.text = itemInfo.name + "\n" + "\n" + itemInfo.description;
    }
}
