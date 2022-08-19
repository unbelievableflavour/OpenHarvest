using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Definitions;

public class AnimalInformation : MonoBehaviour
{
    [Header("Type")]
    public AnimalTypes animalType;
    public string foodColliderName = "PigFood";
    public int index = 0;

    [Header("Models")]
    public GameObject fedTile;
    public GameObject petYoung;
    public GameObject pet;
    public GameObject graveStone;

    [Header("Labeling")]

    public Text nameValue;
    public Text ageValue;
    public Slider hungryMeter;
    public Text hungryValue;
    public GameObject diedLabel;

    public enum AnimalTypes
    {
        Chicken,
        Cow,
        Sheep,
        Pig
    };

    private List<Animal> saveLocation;
    private string petId;
    private bool isDead = false;

    void Start()
    {
        saveLocation = getSaveLocation();
        petId = getPetId();

        if (saveLocation.Count > index && saveLocation[index] != null)
        {
            TimeController.Instance.ListenToDayChange(handleNewDayStarted);
            UpdatePetInformation();
            return;
        }

        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        var collider_tag = other.tag;

        if (collider_tag == "Seed" && other.name == foodColliderName)
        {
            TimeSpan timeSpanLastFed = (TimeSpan)(TimeController.getCurrentTime() - saveLocation[index].lastTimeFedTimestamp);
            if (timeSpanLastFed.Days != 0 && !isDead)
            {
                saveLocation[index].lastTimeFedTimestamp = TimeController.getCurrentTime().Date;
                AudioManager.Instance.PlayClip("seed");
                UpdatePetInformation();
                DecreaseSeedAmount(other.gameObject);
            }
            return;
        }
    }

    private void handleNewDayStarted(object sender, EventArgs e)
    {
        UpdatePetInformation();
    }

    void UpdatePetInformation()
    {
        nameValue.text = saveLocation[index].name;
        TimeSpan timeSpan = (TimeSpan)(TimeController.getCurrentTime() - saveLocation[index].bornTimestamp);
        TimeSpan timeSpanLastFed = (TimeSpan)(TimeController.getCurrentTime() - saveLocation[index].lastTimeFedTimestamp);

        fedTile.SetActive(timeSpanLastFed.Days == 0);
        var hungryFloat = (float)timeSpanLastFed.Days / 10;
        hungryMeter.value = hungryFloat;
        setHungryText(timeSpanLastFed.Days);

        if (timeSpanLastFed.Days >= 10)
        {
            SpawnDeadVersion();
            return;
        }


        ageValue.text = timeSpan.Days.ToString();

        if (timeSpan.Days < 10) //if younger than 3 days
        {
            pet.SetActive(false);
            petYoung.SetActive(true);
            return;
        }

        pet.SetActive(true);
        petYoung.SetActive(false);
    }

    public void SpawnDeadVersion()
    {
        pet.SetActive(false);
        petYoung.SetActive(false);

        isDead = true;
        diedLabel.SetActive(true);
        graveStone.SetActive(true);
        hungryMeter.gameObject.SetActive(false);
    }

    public void removeAnimal()
    {
        TimeController.Instance.RemoveFromDayChange(handleNewDayStarted);
        saveLocation[index] = null;
        GameState.Instance.unlockables[petId]--;
        gameObject.SetActive(false);
    }

    private List<Animal> getSaveLocation()
    {
        if (animalType == AnimalTypes.Cow)
        {
            return GameState.Instance.animals["cows"];
        }

        if (animalType == AnimalTypes.Sheep)
        {
            return GameState.Instance.animals["sheep"];
        }

        if (animalType == AnimalTypes.Pig)
        {
            return GameState.Instance.animals["pigs"];
        }

        return GameState.Instance.animals["chickens"];
    }

    private string getPetId()
    {
        return animalType.ToString();
    }

    public void setHungryText(int hungryInt)
    {
        if (hungryInt >= 10)
        {
            hungryValue.text = "";
            return;
        }

        if (hungryInt > 7)
        {
            hungryValue.text = "yes";
            return;
        }

        if (hungryInt > 3)
        {
            hungryValue.text = "medium";
            return;
        }

        hungryValue.text = "no";
    }
    private void DecreaseSeedAmount(GameObject colliderObject)
    {
        if (colliderObject.GetComponent<SeedDropColliderController>())
        {
            colliderObject.GetComponent<SeedDropColliderController>().NotifyParentSeedBag();
        }
    }
}
