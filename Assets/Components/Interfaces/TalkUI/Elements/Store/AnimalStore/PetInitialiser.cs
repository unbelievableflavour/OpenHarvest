using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Definitions;

public class PetInitialiser : MonoBehaviour
{
    public AudioClip buySound;
    private HarvestDataTypes.StoreProduct item;
    public Text nameLabel;
    public StoreItemsLister storeItemsLister;
    public GameObject boughtPetMessage;

    public void SetItem(HarvestDataTypes.StoreProduct item)
    {
        this.item = item;
    }

    public void BuyItem()
    {
        GameState.Instance.DecreaseMoneyByAmount(item.buyPrice);
        BNG.VRUtils.Instance.PlaySpatialClipAt(buySound, transform.position, 0.6f, 1f, 0.05f);
        string unlockableKey = string.IsNullOrWhiteSpace(item.unlockableId) ? item.id : item.unlockableId;
        if (!string.IsNullOrWhiteSpace(unlockableKey))
        {
            GameState.Instance.unlock(unlockableKey, 1);
        }

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
        string animalId = item != null ? item.id : string.Empty;

        if (animalId == "Chicken")
        {
            return GameState.Instance.animals["chickens"];
        }

        if (animalId == "Cow")
        {
            return GameState.Instance.animals["cows"];
        }

        if (animalId == "Sheep")
        {
            return GameState.Instance.animals["sheep"];
        }

        if (animalId == "Pig")
        {
            return GameState.Instance.animals["pigs"];
        }
        return null;
    }
}
