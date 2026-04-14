using UnityEngine;

public class hasRespawner : MonoBehaviour
{
    private ObjectRespawner objectRespawner;

    public void setSpawnerUid(ObjectRespawner objectRespawner)
    {
        this.objectRespawner = objectRespawner;
    }

    public void addToRespawningObjects()
    {
        if (!objectRespawner)
        {
            return;
        }
        var currentDate = TimeController.getCurrentTime().Date;
        var tomorrow = currentDate.AddDays(objectRespawner.daysUntilRespawn);

        var respawningObject = new RespawningObject()
        {
            uid = objectRespawner.getUid(),
            respawnDateTimestamp = tomorrow,
        };

        if (GameState.Instance.respawningObjects.ContainsKey(respawningObject.uid))
        {
            Debug.Log("updated instead of added");
            GameState.Instance.respawningObjects[respawningObject.uid] = respawningObject;
        } else {
            GameState.Instance.respawningObjects.Add(respawningObject.uid, respawningObject);
        }

        objectRespawner = null;
    }
}
