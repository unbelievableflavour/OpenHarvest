using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Definitions;

public class BreakableTree : MonoBehaviour
{
    [Header("Models")]
    public GameObject tree;
    public GameObject deadTree;

    [Header("ETC")]
    public ParticleSystem spawnEffect;
    public BreakableObjectController breakableObjectController;
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
            SpawnDeadVersion();
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
            deadTree.SetActive(false);
            breakableObjectController.Reset();
            spawnEffect.Play();
            return;
        }
    }

    public void SpawnDeadVersion()
    {
        tree.SetActive(false);
        deadTree.SetActive(true);
        breakableObjectController.SetCooldownManually();
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
