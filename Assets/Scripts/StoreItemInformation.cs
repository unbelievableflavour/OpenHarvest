using UnityEngine;
using UnityEngine.UI;

public class StoreItemInformation : MonoBehaviour
{
    public Text heading;
    public Text description;
    public ItemPreviewer itemPreviewer;

    private string backupHeading;
    private string backupDescription;

    private static HarvestDataTypes.Item currentItem;
    private static bool informationHasBeenUpdated = false;

    private void Start()
    {
        backupHeading = heading.GetComponent<AutoType>().GetMessage();
        backupDescription = description.GetComponent<AutoType>().GetMessage();
    }

    private void Update()
    {
        if (informationHasBeenUpdated)
        {
            return;
        }

        informationHasBeenUpdated = true;

        if (currentItem == null)
        {
            heading.text = backupHeading;
            description.text = backupDescription;
            if (itemPreviewer)
            {
                itemPreviewer.Spawn(currentItem);
            }
            return;
        }

        heading.text = currentItem.name;
        description.text = currentItem.description;
        if (itemPreviewer)
        {
            itemPreviewer.Spawn(currentItem);
        }
    }

    public static void SetItem(HarvestDataTypes.Item item)
    {
        currentItem = item;
        informationHasBeenUpdated = false;
    }

    public static void Reset()
    {
        currentItem = null;
        informationHasBeenUpdated = false;
    }
}
