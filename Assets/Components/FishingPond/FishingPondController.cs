using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Definitions;

public enum FishingStates
{
    notFishingHere,
    fishingShouldBeInitiated,
    fishing,
    fishOnLine,
    fishGotAway,
};

public class FishingPondController : MonoBehaviour
{
    public GameObject fishingIndicator;
    public Transform toolTipPosition;
    public Text fishingIndicatorText;

    public string fishingSpawnLocationName;

    public GameObject waterExitEffect;
    public AudioClip[] waterExitSounds;

    [Header("Drop Tables")]
    public DropTable dropTable;
    public DropTable dropTableWithYellowLure;
    public DropTable dropTableWithRedLure;

    private float fishingTimeFrom = 5.0f;
    private float fishingTimeTill = 10.0f;
    private float timeBeforeFishGetsAway = 2.0f;
    private string hookTag = "FishingHook";

    private int loaderState = 0;
    private FishingStates m_currentState = FishingStates.notFishingHere;
    private Transform fishingHook;
    private string fishingLureId = "";

    void Update()
    {
        if (m_currentState == FishingStates.notFishingHere)
        {
            return;
        }

        if (m_currentState == FishingStates.fishingShouldBeInitiated)
        {   
            m_currentState = FishingStates.fishing;

            fishingIndicator.SetActive(true);
            toolTipPosition.position = fishingHook.position;
            InvokeRepeating("SetLoader", 0f, 1.0f);
            Invoke("SlowDownHook", 1.0f);
            Invoke("InitiateFishing", Random.Range(fishingTimeFrom, fishingTimeTill));
            return;
        }
    }

    void InitiateFishing()
    {
        if (m_currentState == FishingStates.fishing)
        {
            m_currentState = FishingStates.fishOnLine;
            fishingIndicatorText.text = "!";
            if (fishingHook)
            {
                CancelInvoke("SlowDownHook");
                fishingHook.GetComponent<Rigidbody>().drag = 1;
            }
            Invoke("InitiateTooLate", timeBeforeFishGetsAway);
        }
    }

    void InitiateTooLate()
    {
        if (m_currentState == FishingStates.fishOnLine)
        {
            m_currentState = FishingStates.fishGotAway;
            fishingIndicatorText.text = "Too late";
            if (fishingHook)
            {
                SlowDownHook();
            }
        }
    }

    void SetLoader()
    {
        if(m_currentState == FishingStates.fishOnLine)
        {
            CancelInvoke("SetLoader");
            return;
        }

        if(loaderState == 0)
        {
            fishingIndicatorText.text = ".";
            loaderState = 1;
            return;
        }

        if (loaderState == 1)
        {
            fishingIndicatorText.text = "..";
            loaderState = 2;
            return;
        }

        if (loaderState == 2)
        {
            fishingIndicatorText.text = "...";
            loaderState = 0;
            return;
        }
    }

    void SlowDownHook()
    {
        fishingHook.GetComponent<Rigidbody>().drag = 50;
    }

    void OnTriggerEnter(Collider other)
    {
        var collider_tag = other.tag;

        if (collider_tag != hookTag)
        {
            return;
        }

        var hook = other.GetComponent<Hook>();
        if (hook == null)
        {
            return;
        }

        var slot = other.GetComponent<Hook>().snapZone;
        if (slot.HeldItem && !itemIsLure(slot.HeldItem))
        {
            showErrorMessage("This item cannot be used as lure.");
            return;
        }

        if (itemIsLure(slot.HeldItem))
        {
            var item = Definitions.GetItemFromObject(slot.HeldItem);
            fishingLureId = item.itemId;
        }

        fishingHook = other.transform;
        m_currentState = FishingStates.fishingShouldBeInitiated;
    }

    void OnTriggerExit(Collider other)
    {
        var collider_tag = other.tag;

        if (collider_tag != hookTag)
        {
            return;
        }

        if (fishingHook)
        {
            CancelInvoke("SlowDownHook");
            CancelInvoke("InitiateFishing");
            CancelInvoke("InitiateTooLate");
            fishingHook.GetComponent<Rigidbody>().drag = 1;
        }

        if (m_currentState == FishingStates.fishOnLine)
        {
            GameObject randomlyChoosenFish = PickRandomFishFromDropTable();
            var spawnedFish = Instantiate(randomlyChoosenFish);
            var hook = other.GetComponent<Hook>();
            if (hook == null)
            {
                return;
            }

            var slot = other.GetComponent<Hook>().snapZone;
            if (slot.HeldItem)
            {
                Destroy(slot.HeldItem.gameObject);
                slot.HeldItem = null;
            }
            slot.GrabGrabbable(spawnedFish.GetComponent<Grabbable>());
            slot.HeldItem = spawnedFish.GetComponent<Grabbable>();

            if (waterExitEffect)
            {
                var particle = Instantiate(waterExitEffect, other.transform.position, other.transform.rotation);
                Destroy(particle, 2.0f);
            }

            PlayRandomWaterExitSound(other.transform);
        }

        fishingLureId = "";
        m_currentState = FishingStates.notFishingHere;
        fishingIndicator.SetActive(false);
        CancelInvoke("SetLoader");
        return;
    }

    public void PlayRandomWaterExitSound(Transform playLocation)
    {
        if (waterExitSounds.Length != 0)
        {
            VRUtils.Instance.PlaySpatialClipAt(waterExitSounds[Random.Range(0, waterExitSounds.Length)], playLocation.position, 1f, 1f);
        }
    }

    bool itemIsLure(Grabbable item) {
        if (!item || item == null)
        {
            return false;
        }
        return item.GetComponent<FishingLure>() != null;
    }

    GameObject PickRandomFishFromDropTable() {
        if (fishingLureId == "RedLure" && dropTableWithYellowLure.items.Count != 0)
        {
            return dropTableWithRedLure.GetItemByDropRate();
        }

        if (fishingLureId == "YellowLure" && dropTableWithYellowLure.items.Count != 0)
        {
            return dropTableWithYellowLure.GetItemByDropRate();
        }

        return dropTable.GetItemByDropRate();
    }

    void showErrorMessage(string errorText)
    {
        fishingIndicator.SetActive(true);
        fishingIndicatorText.text = errorText;       
    }
}
