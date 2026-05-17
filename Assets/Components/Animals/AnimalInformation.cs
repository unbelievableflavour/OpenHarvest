using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Definitions;

public class AnimalInformation : MonoBehaviour
{
    [Header("Type")]
    public string foodColliderName = "PigFood";

    [Header("Plateau")]
    public GameObject petPrefab;
    public Transform petSpawnPoint;
    public GameObject fedTile;
    public GameObject graveStone;

    [Header("Labeling")]
    public Text nameValue;
    public Text ageValue;
    public Slider hungryMeter;
    public Text hungryValue;
    public GameObject diedLabel;

    public string plateauInstanceId { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsFollowing => !string.IsNullOrEmpty(plateauInstanceId) &&
                               GameState.Instance?.followingNpcInstanceId == plateauInstanceId;
    public Animal CurrentAnimal => animal;

    public event Action<Animal> OnAnimalRefreshed;

    private Animal animal;
    private GameObject petInstance;

    void Start()
    {
        plateauInstanceId = GetComponentInParent<PlacedObjectInstanceId>(true)?.instanceId ?? string.Empty;
        RefreshAnimal();
    }

    void OnEnable()
    {
        if (string.IsNullOrEmpty(plateauInstanceId))
        {
            return;
        }

        RefreshAnimal();
    }

    void OnDisable()
    {
        TimeController.Instance?.RemoveFromDayChange(handleNewDayStarted);
    }

    void OnDestroy()
    {
        TimeController.Instance?.RemoveFromDayChange(handleNewDayStarted);
    }

    public void RefreshAnimal()
    {
        animal = GameState.Instance?.animals?.FirstOrDefault(a => a.plateauInstanceId == plateauInstanceId);

        if (animal == null && !string.IsNullOrEmpty(plateauInstanceId))
        {
            animal = new Animal
            {
                plateauInstanceId = plateauInstanceId,
                name = "Bob",
                bornTimestamp = TimeController.getCurrentTime().Date,
                lastTimeFedTimestamp = TimeController.getCurrentTime().Date,
            };
            GameState.Instance.animals.Add(animal);
        }

        if (animal != null)
        {
            TimeController.Instance.ListenToDayChange(handleNewDayStarted);
            UpdatePetInformation();
        }
    }

    public void SetAnimalName(string newName)
    {
        if (animal == null || string.IsNullOrWhiteSpace(newName))
        {
            return;
        }

        animal.name = newName.Trim();
        if (nameValue != null) nameValue.text = animal.name;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Seed" || other.name != foodColliderName || animal == null)
        {
            return;
        }

        TimeSpan timeSpanLastFed = (TimeSpan)(TimeController.getCurrentTime() - animal.lastTimeFedTimestamp);
        if (timeSpanLastFed.Days != 0 && !IsDead)
        {
            animal.lastTimeFedTimestamp = TimeController.getCurrentTime().Date;
            AudioManager.Instance.PlayClip("seed");
            UpdatePetInformation();
            DecreaseSeedAmount(other.gameObject);
        }
    }

    private void handleNewDayStarted(object sender, EventArgs e)
    {
        UpdatePetInformation();
    }

    void UpdatePetInformation()
    {
        DoUpdatePetInformation();
        OnAnimalRefreshed?.Invoke(animal);
    }

    void DoUpdatePetInformation()
    {
        if (animal == null)
        {
            return;
        }

        if (nameValue != null) nameValue.text = animal.name;
        TimeSpan timeSpan = (TimeSpan)(TimeController.getCurrentTime() - animal.bornTimestamp);
        TimeSpan timeSpanLastFed = (TimeSpan)(TimeController.getCurrentTime() - animal.lastTimeFedTimestamp);

        fedTile.SetActive(timeSpanLastFed.Days == 0);
        hungryMeter.value = (float)timeSpanLastFed.Days / 10;
        setHungryText(timeSpanLastFed.Days);

        if (timeSpanLastFed.Days >= 10)
        {
            SpawnDeadVersion();
            return;
        }

        ageValue.text = timeSpan.Days.ToString();

        if (!IsFollowing)
        {
            EnsurePetSpawned();
        }
    }

    private void EnsurePetSpawned()
    {
        if (petInstance == null && petPrefab != null)
        {
            Transform spawnAt = petSpawnPoint != null ? petSpawnPoint : transform;
            petInstance = Instantiate(petPrefab, spawnAt.position, spawnAt.rotation);


            petInstance.GetComponentInChildren<DetermineModelByAge>(true)?.SetId(plateauInstanceId);
        }
    }


    public void SpawnDeadVersion()
    {
        IsDead = true;
        diedLabel.SetActive(true);
        graveStone.SetActive(true);
        hungryMeter.gameObject.SetActive(false);
    }

    public void removeAnimal()
    {
        TimeController.Instance.RemoveFromDayChange(handleNewDayStarted);
        GameState.Instance.animals.RemoveAll(a => a.plateauInstanceId == plateauInstanceId);
        PlacementSystem.Instance?.RemovePlacedObject(gameObject);
    }

    public void setHungryText(int hungryInt)
    {
        if (hungryInt >= 10) { hungryValue.text = ""; return; }
        if (hungryInt > 7)   { hungryValue.text = "yes"; return; }
        if (hungryInt > 3)   { hungryValue.text = "medium"; return; }
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
