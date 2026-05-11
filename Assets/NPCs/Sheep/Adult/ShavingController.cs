using BNG;
using System;
using UnityEngine;

public class ShavingController : MonoBehaviour
{
    public ParticleSystem spawnEffect;
    public GameObject wool1;
    public GameObject wool2;
    public GameObject wool3;
    public GameObject wool4;

    private string uid;

    private void Start()
    {
        uid = GetComponent<UniqueId>().uniqueId;
        TimeController.Instance.ListenToDayChange(handleNewDayStarted);

        RespawningObject respawningObject = findRespawningObjectByUid(uid);

        if (respawningObject != null)
        {
            if (TimeController.getCurrentTime().Date >= respawningObject.respawnDateTimestamp)
            {
                GameState.Instance.respawningObjects.Remove(uid);
                SetWoolActive();
                return;
            }
            return;
        }

        SetWoolActive();
    }

    private void handleNewDayStarted(object sender, EventArgs e)
    {
        ResetWool();
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

    public void addToRespawningObjects()
    {
        if (GameState.Instance.respawningObjects.ContainsKey(uid))
        {
            Debug.Log("already respawning");
            return;
        }

        var currentDate = TimeController.getCurrentTime().Date;
        var tomorrow = currentDate.AddDays(1);

        var respawningObject = new RespawningObject()
        {
            uid = uid,
            respawnDateTimestamp = tomorrow,
        };

        GameState.Instance.respawningObjects.Add(respawningObject.uid, respawningObject);
    }

    void SetWoolActive()
    {
        wool1.SetActive(true);
        wool2.SetActive(true);
        wool3.SetActive(true);
        wool4.SetActive(true);
    }

    void ResetWool()
    {
        bool atLeast1Respawned = false;

        wool1.SetActive(true);
        var breakable1 = wool1.GetComponent<BreakableObjectController>();
        if (breakable1.isAlreadyBroken())
        {
            atLeast1Respawned = true;
            breakable1.Reset();
        }

        wool2.SetActive(true);
        var breakable2 = wool2.GetComponent<BreakableObjectController>();
        if (breakable2.isAlreadyBroken())
        {
            atLeast1Respawned = true;
            breakable2.Reset();
        }

        wool3.SetActive(true);
        var breakable3 = wool3.GetComponent<BreakableObjectController>();
        if (breakable3.isAlreadyBroken())
        {
            atLeast1Respawned = true;
            breakable3.Reset();
        }

        wool4.SetActive(true);
        var breakable4 = wool4.GetComponent<BreakableObjectController>();
        if (breakable4.isAlreadyBroken())
        {
            atLeast1Respawned = true;
            breakable4.Reset();
        }


        if (atLeast1Respawned)
        {
            spawnEffect.Play();
        }
    }
}
