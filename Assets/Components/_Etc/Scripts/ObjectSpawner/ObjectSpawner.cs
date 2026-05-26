using BNG;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public bool useSpawnLocationAsParent = true;
    public bool rotateOnSpawn = true;
    public bool rotateOnSpawnZ = false;
    public GameObject spawnLocations;

    [Header("Droptable is used when added")]
    public DropTable dropTable;
    public int spawnChangeInPercentage = 70;

    private SpawnManager spawnManager;
    private ObjectRespawner objectRespawner;

    public void Awake()
    {
        spawnManager = GetComponent<SpawnManager>();
        objectRespawner = GetComponent<ObjectRespawner>();
    }

    public void SpawnFruit()
    {
        foreach (Transform spawnLocation in spawnLocations.transform)
        {
            if (spawnLocation.childCount > 0)
            {
                var slot = spawnLocation.GetComponent<SnapZone>();
                if (!slot || slot.HeldItem)
                {
                    return;
                }
            }

            var numberThatShouldBeBelowPercentage = Random.Range(0, 100);
            if (numberThatShouldBeBelowPercentage <= spawnChangeInPercentage)
            {
                Spawn(spawnLocation);
            }
        }
    }

    public void SpawnSpecificNumberOfFruits(int numberOfFruits)
    {
        RemoveAllSpawnedObjects();
        int counter = 0;
        foreach (Transform spawnLocation in spawnLocations.transform)
        {
            if (counter == numberOfFruits)
            {
                return;
            }

            Spawn(spawnLocation);
            counter++;
        }
    }

    public void RemoveAllSpawnedObjects()
    {
        foreach (Transform spawnLocation in spawnLocations.transform)
        {
            foreach (Transform fruit in spawnLocation)
            {
                // this is just the ringhelper
                if(!fruit.GetComponent<ItemInformation>()) {
                    continue;
                }
                Destroy(fruit.gameObject);
            }
        }
    }

    public int GetCurrentNumberOfSpawnedObjects()
    {
        int numberOfSpawnedFruits = 0;
        foreach (Transform spawnLocation in spawnLocations.transform)
        {
            if (spawnLocation.childCount > 0)
            {
                var slot = spawnLocation.GetComponent<SnapZone>();
                if (!slot || slot.HeldItem)
                {
                    numberOfSpawnedFruits++;
                }
            }
        }

        return numberOfSpawnedFruits;
    }


    public void Spawn(Transform spawnLocation)
    { 
        var spawnedObject = Instantiate(dropTable.GetItemByDropRate(), spawnLocation.position, spawnLocation.rotation);
        if (rotateOnSpawn)
        {
            spawnedObject.transform.Rotate(0, Random.Range(0f, 360f), 0);
        }

        if (rotateOnSpawnZ)
        {
            spawnedObject.transform.Rotate(0, 0, Random.Range(0f, 360f));
        }

        var hasSpawnManager = spawnedObject.GetComponent<HasSpawnManager>();
        if (hasSpawnManager)
        {
            hasSpawnManager.SetOriginatedSpawnManager(spawnManager);
        }

        var hasRespawner = spawnedObject.GetComponent<hasRespawner>();
        if (hasRespawner && objectRespawner)
        {
            hasRespawner.setSpawnerUid(objectRespawner);
        }

        if (!useSpawnLocationAsParent)
        {
            return;
        }

        var slot = spawnLocation.GetComponent<SnapZone>();
        if (slot)
        {
            var temporaryItemGrabbable = spawnedObject.GetComponent<Grabbable>();
            if (!temporaryItemGrabbable)
            {
                return;
            }
            slot.GrabGrabbable(temporaryItemGrabbable);
            slot.HeldItem = temporaryItemGrabbable;
        }
        else
        {
            spawnedObject.transform.SetParent(spawnLocation);
        }
    }
}
