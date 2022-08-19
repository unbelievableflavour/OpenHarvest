using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Definitions;

public class ChickenInformation : MonoBehaviour
{
    public int index = 0;

    public GameObject fedTile;
    public GameObject petYoung;
    public GameObject pet;

    public Text nameValue;
    public Text ageValue;
    public Slider hungryMeter;
    public Text hungryValue;
    public GameObject graveStone;
    public GameObject diedLabel;

    //Chicken specific
    public GameObject feather;
    public GameObject eggsPlateau;

    private List<Animal> saveLocation;
    private bool isDead = false;

    void Start()
    {
        saveLocation = GameState.Instance.animals["chickens"];
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

        if (collider_tag == "Seed" && other.name == "ChickenFood")
        {
            TimeSpan timeSpanLastFed = (TimeSpan)(TimeController.getCurrentTime() - saveLocation[index].lastTimeFedTimestamp);
            if (timeSpanLastFed.Days != 0 && !isDead) {
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
        TimeSpan timeSpanAge = (TimeSpan)(TimeController.getCurrentTime() - saveLocation[index].bornTimestamp);
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

        ageValue.text = timeSpanAge.Days.ToString();        

        if (timeSpanAge.Days < 10) //if younger than 3 days
        {
            pet.SetActive(false);
            petYoung.SetActive(true);
            feather.SetActive(false);
            eggsPlateau.SetActive(false);
            return;
        }

        pet.SetActive(true);
        petYoung.SetActive(false);
        feather.SetActive(true);
        eggsPlateau.SetActive(true);
    }

    public void SpawnDeadVersion()
    {
        pet.SetActive(false);
        petYoung.SetActive(false);
        feather.SetActive(false);
        eggsPlateau.SetActive(false);

        isDead = true;
        diedLabel.SetActive(true);
        graveStone.SetActive(true);
        hungryMeter.gameObject.SetActive(false);
    }

    public void removeAnimal()
    {
        TimeController.Instance.RemoveFromDayChange(handleNewDayStarted);
        saveLocation[index] = null;
        GameState.Instance.unlockables["Chicken"]--;
        gameObject.SetActive(false);
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
