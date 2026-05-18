using System;
using UnityEngine;

/// <summary>
/// Add alongside <see cref="AnimalInformation"/> on ChickenPlateau prefabs.
/// Handles feather and egg plateau visibility — extras that only chickens have.
/// </summary>
public class ChickenExtras : MonoBehaviour
{
    public GameObject feather;
    public GameObject eggsPlateau;

    private AnimalInformation animalInfo;

    void Start()
    {
        animalInfo = GetComponent<AnimalInformation>();
        animalInfo.OnAnimalRefreshed += Refresh;
        Refresh(animalInfo.CurrentAnimal);
    }

    void OnDestroy()
    {
        if (animalInfo != null)
        {
            animalInfo.OnAnimalRefreshed -= Refresh;
        }
    }

    void Refresh(Animal animal)
    {
        bool isAdult = animal != null
            && !animalInfo.IsDead
            && !animalInfo.IsFollowing
            && ((TimeSpan)(TimeController.getCurrentTime() - animal.bornTimestamp)).Days >= 10;

        feather.SetActive(isAdult);
        eggsPlateau.SetActive(isAdult);
    }
}
