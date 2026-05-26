using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    public SoilBehaviour soilBehaviour;

    GameObject spawnedPlant;
    private string m_selectedPlant = "";
    private Dictionary<string, GameObject> plantdictionary = new Dictionary<string, GameObject>();

     [System.Serializable]
    public struct PlantManagerPlant
    {
        public string name;
        public GameObject plant;
    }

    public PlantManagerPlant[] newplantPrefabs;

    public void Awake()
    {
        foreach(var plant in newplantPrefabs)
        {
            plantdictionary.Add(plant.name, plant.plant);
        }
    }

    public void SpawnPlant(string plantType)
    {
        m_selectedPlant = plantType;

        if (!plantdictionary.ContainsKey(m_selectedPlant))
        {
            return;
        }

        spawnedPlant = Instantiate(plantdictionary[m_selectedPlant], transform.position, transform.rotation);
        spawnedPlant.transform.SetParent(transform);
        if (!spawnedPlant.GetComponent<DisableRandomPlantRotation>()) {
           spawnedPlant.transform.Rotate(0, Random.Range(0f, 360f), 0);
        }
        
        spawnedPlant.GetComponent<GrowthState>().SetSoilBehaviour(soilBehaviour);
    }

    public void Reset()
    {
        if (spawnedPlant) {
            Destroy(spawnedPlant);
        }

        m_selectedPlant = "";
    }

    public void Water()
    {
        if (spawnedPlant)
        {
            spawnedPlant.GetComponent<GrowthState>().Water();
        }
    }

    public string getSelectedPlant()
    {
        return m_selectedPlant.ToString();
    }

    public GrowthState getSelectedPlantGrowthState()
    {
        if (!spawnedPlant)
        {
            return null;
        }

        var growthState = spawnedPlant.GetComponent<GrowthState>();

        if (!growthState)
        {
            return null;
        }

        return growthState;
    }

    public ObjectSpawner getSelectedPlantObjectSpawner()
    {
        if (!spawnedPlant)
        {
            return null;
        }

        var objectSpawner = spawnedPlant.GetComponent<ObjectSpawner>();

        if (!objectSpawner)
        {
            return null;
        }

        return objectSpawner;
    }
}
