using System;
using UnityEngine;

public class BreakableDeadTree : MonoBehaviour
{
    [Header("Models")]
    public GameObject tree;

    [Header("ETC")]
    public ParticleSystem spawnEffect;
    public int daysUntilRespawn = 1;
    private string uid;

    void Start()
    {
        uid = GetComponent<UniqueId>().uniqueId;
        TimeController.Instance.ListenToDayChange(handleNewDayStarted);
        RespawningObject respawningObject = findRespawningObjectByUid(uid);

        if (respawningObject != null)
        {
            if (TimeController.getCurrentTime().Date >= respawningObject.respawnDateTimestamp)
            {
                GameState.Instance.respawningObjects.Remove(uid);
                ResetTree();
                return;
            }
            return;
        }
    }

    private void handleNewDayStarted(object sender, EventArgs e)
    {
        ResetTree();
    }

    public void ResetTree()
    {
        if (tree.activeSelf)
        {
            return;
        }

        RespawningObject respawningObject = findRespawningObjectByUid(uid);

        if (TimeController.getCurrentTime().Date >= respawningObject.respawnDateTimestamp)
        {
            GameState.Instance.respawningObjects.Remove(uid);
            tree.SetActive(true);
            spawnEffect.Play();
            return;
        }
    }

    public RespawningObject findRespawningObjectByUid(string uid)
    {
        bool keyExists = GameState.Instance.respawningObjects.ContainsKey(uid);
        if (!keyExists)
        {
            return null;
        }

        return GameState.Instance.respawningObjects[uid];
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
