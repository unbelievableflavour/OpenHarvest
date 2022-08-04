using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Definitions;

public class PetInitialiser : MonoBehaviour
{
    public AudioClip buySound;
    private Item item;
    public Text nameLabel;
    public StoreItemsLister storeItemsLister;
    public GameObject boughtPetMessage;

    public void SetItem(Item item)
    {
        this.item = item;
    }

    public void BuyItem()
    {
        GameState.DecreaseMoneyByAmount(item.buyPrice);
        BNG.VRUtils.Instance.PlaySpatialClipAt(buySound, transform.position, 0.6f, 1f, 0.05f);
        GameState.unlockables[item.itemId]++;

        storeItemsLister.RefreshStoreRows();
        gameObject.SetActive(false);
        boughtPetMessage.SetActive(true);

        var saveLocation = getSaveLocation();
        if(saveLocation == null)
        {
            Debug.Log("couldn't find the save location for this animal");
        }

        var pet = (new Animal()
        {
            name = nameLabel.text,
            bornTimestamp = TimeController.getCurrentTime().Date,
            lastTimeFedTimestamp = TimeController.getCurrentTime().Date,
        });

        int index = saveLocation.IndexOf(null);
        if (index != -1) // If there are deleted chickens, use their spot!
        {
            saveLocation[index] = pet;
        } else {
            saveLocation.Add(pet);
        }
            
        nameLabel.text = "Bob";

        return;
    }

    private List<Animal> getSaveLocation()
    {
        if (item.itemId == "Chicken")
        {
            return GameState.animals["chickens"];
        }

        if (item.itemId == "Cow")
        {
            return GameState.animals["cows"];
        }

        if (item.itemId == "Sheep")
        {
            return GameState.animals["sheep"];
        }

        if (item.itemId == "Pig")
        {
            return GameState.animals["pigs"];
        }
        return null;
    }
}
