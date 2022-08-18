using UnityEngine;
using UnityEngine.UI;

public class TooltipInformation : MonoBehaviour
{
    public Text textComponent;

    void Start()
    {
        if (!textComponent)
        {
            return;
        }

        var item = Definitions.GetItemFromObject(transform.parent);
        if (item == null)
        {
            return;
        }

        textComponent.text = item.name + "\n" + "\n" + item.description;
    }
}
