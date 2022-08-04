using BNG;
using UnityEngine;

public class SpawnItemInSlot : MonoBehaviour
{
    public string itemId = "";

    // Start is called before the first frame update
    void Start()
    {
        var itemInfo = Definitions.GetItemInformation(itemId);
        var instantiatedItem = Definitions.InstantiateItem(itemInfo.prefabFileName);
        var instantiatedItemGrabbable = instantiatedItem.GetComponent<Grabbable>();
        GetComponent<SnapZone>().GrabGrabbable(instantiatedItemGrabbable);
    }
}
