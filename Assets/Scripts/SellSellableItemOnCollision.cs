using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SellSellableItemOnCollision : MonoBehaviour
{
    public AudioSource gainMoneySound;
    public GameObject text;
    public ParticleSystem sellEffect;

    private SlidingBuffer<GameObject> soldGameObjects;

    void Start()
    {
        soldGameObjects = new SlidingBuffer<GameObject>(5);
    }

    void OnTriggerEnter(Collider other)
    {
        var item = Definitions.GetItemFromObject(other.gameObject);
        if(!item || !item.isSellable) {
            return;
        }

        if (soldGameObjects.Contains(other.gameObject)){ //if another collider of the object already entered the trigger
            return;
        }

        soldGameObjects.Add(other.gameObject);

        var itemStack = other.GetComponent<ItemStack>();
        var sellingPrice = itemStack ? GetSellingPrice(item) * itemStack.GetStackSize() : GetSellingPrice(item);
 
        GameState.Instance.IncreaseMoneyByAmount(sellingPrice);
        text.GetComponent<Text>().text = "+" + sellingPrice;       

        gainMoneySound.Play();
        sellEffect.Play();
        Destroy(other.gameObject);
        text.SetActive(true);
        StartCoroutine(DisableMoneyGainedText());
    }

    public int GetSellingPrice(HarvestDataTypes.Item item)
    {
        int sellPrice = item.sellPrice;

        foreach (var itemOfTheWeek in GameState.Instance.itemsOfTheWeek)
        {
            if (item.itemId == itemOfTheWeek.Value.currentItemId)
            {
                return sellPrice * 2;
            }
        }

        return sellPrice;
    }

    IEnumerator DisableMoneyGainedText()
    {
        yield return new WaitForSeconds(2f);

        text.SetActive(false);
    }
}
