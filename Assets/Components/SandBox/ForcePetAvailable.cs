using System.Collections.Generic;
using UnityEngine;

public class ForcePetAvailable : MonoBehaviour
{
    public enum AnimalTypes
    {
        Chicken,
        Cow,
        Sheep,
        Pig
    };

    public AnimalTypes animalType;
    public string animalName = "Bob";
    public int age = 0;

    void Awake()
    {
        var saveLocation = getSaveLocation();
        var pet = new Animal()
        {
            name = animalName,
            bornTimestamp = TimeController.getCurrentTime().AddDays(age),
            lastTimeFedTimestamp = TimeController.getCurrentTime().AddDays(age),
        };

        int index = saveLocation.IndexOf(null);
        if (index != -1)
        {
            saveLocation[index] = pet;
        } else {
            saveLocation.Add(pet);
        }
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
}
