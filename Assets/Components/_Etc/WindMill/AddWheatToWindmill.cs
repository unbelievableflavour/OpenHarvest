using UnityEngine;

public class AddWheatToWindmill : MonoBehaviour
{
    public WindmillController windmillController;
    public AudioSource sound;
    public ParticleSystem effect;

    private SlidingBuffer<GameObject> alreadyGrindedGameObjects;

    private string WheatId = "Wheat";

    void Start()
    {
        alreadyGrindedGameObjects = new SlidingBuffer<GameObject>(5);
    }

    void OnTriggerEnter(Collider other)
    {
        var item = Definitions.GetItemFromObject(other.gameObject);
        if (!item)
        {
            return;
        }

        if (item.itemId != WheatId)
        {
            return;
        }

        if (alreadyGrindedGameObjects.Contains(other.gameObject)) { //if another collider of the object already entered the trigger
            return;
        }

        alreadyGrindedGameObjects.Add(other.gameObject);

        var itemStack = other.GetComponent<ItemStack>();

        windmillController.AddWheat(itemStack ? itemStack.GetStackSize() : 1);
        sound.Play();
        effect.Play();
        Destroy(other.gameObject);
    }
}
