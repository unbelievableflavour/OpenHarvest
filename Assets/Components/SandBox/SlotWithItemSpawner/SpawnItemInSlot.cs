using BNG;
using UnityEngine;

public class SpawnItemInSlot : MonoBehaviour
{
    public HarvestDataTypes.Item item;

    // Start is called before the first frame update
    void Start()
    {
        var instantiatedItem = Definitions.InstantiateItemNew(item.prefab);
        var instantiatedItemGrabbable = instantiatedItem.GetComponent<Grabbable>();
        GetComponent<SnapZone>().GrabGrabbable(instantiatedItemGrabbable);
    }
}
