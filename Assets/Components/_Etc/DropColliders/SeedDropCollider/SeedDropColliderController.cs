using UnityEngine;

public class SeedDropColliderController : MonoBehaviour
{
    private SpawnSeedsWhenHoldingUpsideDown parentSeedBag;

    public void setParentSeedbag(SpawnSeedsWhenHoldingUpsideDown newParentSeedBag)
    {
        parentSeedBag = newParentSeedBag;
    }

    public void NotifyParentSeedBag()
    {
        parentSeedBag.DecreaseStack();
    }
}