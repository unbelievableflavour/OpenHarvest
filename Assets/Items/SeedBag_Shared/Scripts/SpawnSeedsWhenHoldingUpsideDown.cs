using BNG;
using UnityEngine;
using static Definitions;
using static PlantManager;

public class SpawnSeedsWhenHoldingUpsideDown: MonoBehaviour
{
    public GameObject spawnedObject;
    public GameObject dropSeedsParticle;
    public Transform spawnLocation;
    public Transform bottomOfBag;
    public string spawnedObjectTag;
    private string tag = "Seed";

    private Grabbable grabbable;
    private float timer;

    private void Start()
    {
        grabbable = GetComponent<Grabbable>();
    }

    void Update()
    {
        if (grabbable && !grabbable.BeingHeld)
        {
            dropSeedsParticle.SetActive(false);
            return;
        }

        timer += Time.deltaTime;
        if (timer < 0.5)
        {
            return;
        }
        timer = 0;

        if (bottomOfBag.transform.position.y > spawnLocation.transform.position.y)
        {
            dropSeedsParticle.SetActive(true);
            GameObject droppedSeed = Instantiate(spawnedObject, spawnLocation.position, spawnLocation.rotation);
            droppedSeed.GetComponent<SeedDropColliderController>().setParentSeedbag(this);
            droppedSeed.tag = tag;
            droppedSeed.name = spawnedObjectTag.ToString();
            Destroy(droppedSeed, 2.0f);
            return;
        }
        dropSeedsParticle.SetActive(false);
    }

    public void DecreaseStack()
    {
        var stack = GetComponent<ItemStack>();
        if (!stack)
        {
            return;
        }

        var currentItemStackSize = stack.GetStackSize() - 1;
        stack.SetStackSize(currentItemStackSize);

        if(currentItemStackSize < 1)
        {
            var grabbable = GetComponent<Grabbable>();
            grabbable.DropItem(false, true);
            Destroy(gameObject);
        }
    }
}
