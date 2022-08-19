using BNG;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Definitions;

public class MilkingAreaController : MonoBehaviour
{
    public GameObject tooltip;
    public Text tooltipText;
    public SnapZone bucketSnapZone;
    public GameObject doneVersion;
    public HarvestDataTypes.Item bucketWithMilk;

    private int loaderState = 0;
    private bool bucketIsSnapped = false;
    private bool isSpawningMilkBucket = false;

    private string uid;

    bool hasMilk = false;

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
                hasMilk = true;
                return;
            }
            return;
        }

        hasMilk = true;
    }

    private void handleNewDayStarted(object sender, EventArgs e)
    {
        hasMilk = true;
        SnapBucket();
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

    void SetLoader()
    {
        if (loaderState == 0)
        {
            tooltipText.text = ".";
            loaderState = 1;
            return;
        }

        if (loaderState == 1)
        {
            tooltipText.text = "..";
            loaderState = 2;
            return;
        }

        if (loaderState == 2)
        {
            tooltipText.text = "...";
            loaderState = 3;
            return;
        }

        if (loaderState == 3)
        {
            tooltipText.text = "!";
            SpawnMilkBucket();
            loaderState = 4;

            hasMilk = false;
            addToRespawningObjects();
            return;
        }
    }

    public void SnapBucket()
    {
        if (isSpawningMilkBucket)
        {
            return;
        }

        if (!bucketSnapZone.HeldItem)
        {
            return;
        }


        if (bucketSnapZone.HeldItem.GetComponent<ItemStack>() && bucketSnapZone.HeldItem.GetComponent<ItemStack>().GetStackSize() > 1)
        {
            tooltip.SetActive(true);
            tooltipText.text = "You can only use 1 bucket";
            return;
        }

        if (!hasMilk)
        {
            tooltip.SetActive(true);
            tooltipText.text = "Cow doesn't have any milk left";
            return;
        }

        var currentItem = Definitions.GetItemFromObject(bucketSnapZone.HeldItem);
        if (currentItem == bucketWithMilk)
        {
            tooltip.SetActive(true);
            tooltipText.text = "Bucket is already full";
            return;
        }

        tooltip.SetActive(true);
        tooltipText.text = "Start pumping for milk!";
        bucketIsSnapped = true;
    }

    public void UnspanBucket()
    {
        if (isSpawningMilkBucket)
        {
            return;
        }

        tooltip.SetActive(false);
        loaderState = 0;
        bucketIsSnapped = false;
        return;
    }

    private void SpawnMilkBucket()
    {
        isSpawningMilkBucket = true;
        var oldGrabObject = bucketSnapZone.HeldItem.gameObject;
        var milkBucket = Instantiate(doneVersion);
        bucketSnapZone.GrabGrabbable(milkBucket.GetComponent<Grabbable>());
        Destroy(oldGrabObject);
        isSpawningMilkBucket = false;
    }

    public void Pump()
    {
        if (!bucketIsSnapped)
        {
            return;
        }

        if(loaderState == 4)
        {
            tooltip.SetActive(true);
            tooltipText.text = "Bucket is already full";
            return;
        }
        SetLoader();
    }

    public void addToRespawningObjects()
    {
        var currentDate = TimeController.getCurrentTime().Date;
        var tomorrow = currentDate.AddDays(1);

        var respawningObject = new RespawningObject()
        {
            uid = uid,
            respawnDateTimestamp = tomorrow,
        };

        GameState.Instance.respawningObjects.Add(respawningObject.uid, respawningObject);
    }
}
