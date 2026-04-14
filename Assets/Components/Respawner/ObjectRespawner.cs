using System;
using UnityEngine;

public class ObjectRespawner : MonoBehaviour
{
    public ParticleSystem spawnEffect;
    public int daysUntilRespawn = 1;

    private string uid;
    private ObjectSpawner objectSpawner;

    void Start()
    {
        objectSpawner = GetComponent<ObjectSpawner>();
        uid = GetComponent<UniqueId>().uniqueId;
        TimeController.Instance.ListenToDayChange(handleNewDayStarted);
        RespawningObject respawningObject = findRespawningObjectByUid(uid);

        if (respawningObject != null)
        {
            if (TimeController.getCurrentTime().Date >= respawningObject.respawnDateTimestamp)
            {
                GameState.Instance.respawningObjects.Remove(uid);
                objectSpawner.SpawnFruit();
                return;
            }
            return;
        }

        objectSpawner.SpawnFruit();
    }

    public void checkForRespawn()
    {
        if(objectSpawner.GetCurrentNumberOfSpawnedObjects() != 0)
        {
            return;
        }

        RespawningObject respawningObject = findRespawningObjectByUid(uid);

        if (TimeController.getCurrentTime().Date >= respawningObject.respawnDateTimestamp)
        {
            GameState.Instance.respawningObjects.Remove(uid);
            objectSpawner.SpawnFruit();
            spawnEffect.Play();
            return;
        }
    }

    public RespawningObject findRespawningObjectByUid(string uid)
    {
        bool keyExists = GameState.Instance.respawningObjects.ContainsKey(uid);
        if (!keyExists) {
            return null;
        }

        return GameState.Instance.respawningObjects[uid];
    }

    private void handleNewDayStarted(object sender, EventArgs e)
    {
        checkForRespawn();
    }

    public string getUid()
    {
        return uid;
    }

    //Copy of the one in hasRespawner
    public void addToRespawningObjects()
    {
        var currentDate = TimeController.getCurrentTime().Date;
        var tomorrow = currentDate.AddDays(daysUntilRespawn);

        var respawningObject = new RespawningObject()
        {
            uid = uid,
            respawnDateTimestamp = tomorrow,
        };

        GameState.Instance.respawningObjects.Add(respawningObject.uid, respawningObject);
    }
}