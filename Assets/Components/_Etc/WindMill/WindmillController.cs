using BNG;
using UnityEngine;
using UnityEngine.UI;
using System;
using static Definitions;

public class WindmillController : MonoBehaviour
{
    public HarvestDataTypes.Item flour;
    public SpawnManager spawnManager;
    public WheatHeightController wheatHeightController;
    public Text updateWheatCountLabel;
    public Text flourReadyLabel;
    public GameObject flourSnapZone;

    private int currentWheatCount = 0;    
    private bool isFlourReady = false;    

    private WindmillStates m_currentState = WindmillStates.Initialized;

    void Start()
    {
        SceneSwitcher.Instance.beforeSceneSwitch += beforeSceneSwitch;
        LoadWindmill();
    }

    public void AddWheat(int addedNumberOfWheat) {
        m_currentState = WindmillStates.Grinding;
        isFlourReady = false;
        currentWheatCount += addedNumberOfWheat;
        UpdatedWheatCountLabel();
        spawnManager.Spawn(true);
        wheatHeightController.TransitionToFull();
    }

    public void SetWheatGrinded()
    {
        m_currentState = WindmillStates.Finished;
        isFlourReady = true;
        UpdatedWheatCountLabel();
    }

    public void DropFlour()
    {
        m_currentState = WindmillStates.Initialized;
        SpawnFlour();
        currentWheatCount = 0;
        isFlourReady = false;
        UpdatedWheatCountLabel();
        wheatHeightController.TransitionToEmpty();
    }

    void SpawnFlour()
    {
        var snapZone = flourSnapZone.GetComponent<SnapZone>();
        if (snapZone.HeldItem != null)
        {
            snapZone.HeldItem.GetComponent<ItemStack>().IncreaseStack(currentWheatCount);
            return;
        }

        GameObject item = Definitions.InstantiateItemNew(flour.prefab);
        flourSnapZone.SetActive(true);
        snapZone.GrabGrabbable(item.GetComponent<Grabbable>());
        snapZone.HeldItem.GetComponent<ItemStack>().SetStackSize(currentWheatCount);
    }

    void UpdatedWheatCountLabel()
    {
        updateWheatCountLabel.text = "Wheat count currently in grinder: " + currentWheatCount.ToString();
        flourReadyLabel.text = "Flour is ready: " + (isFlourReady ? "yes" : "no");
    }

    public int getCurrentWheatCount()
    {
        return currentWheatCount;
    }

    public bool IsFlourReady()
    {
        return isFlourReady;
    }

    void LoadWindmill()
    {
        SaveableWindmill windmill = GameState.Instance.windmill;

        if (windmill == null)
        {
            return;
        }

        if (windmill.currentFlourCount != 0)
        {
            var snapZone = flourSnapZone.GetComponent<SnapZone>();
            GameObject item = InstantiateItemNew(flour.prefab);
            flourSnapZone.SetActive(true);
            snapZone.GrabGrabbable(item.GetComponent<Grabbable>());
            snapZone.HeldItem.GetComponent<ItemStack>().SetStackSize(windmill.currentFlourCount);
        }

        if (windmill.state == WindmillStates.Grinding)
        {
            AddWheat(windmill.currentWheatCount);
            spawnManager.setStartingTimestamp(windmill.startingTimestamp);
            spawnManager.UpdateSpawnManager();
            return;
        }

        if (windmill.state == WindmillStates.Finished)
        {
            currentWheatCount = windmill.currentWheatCount;
            wheatHeightController.TransitionToFull();
            SetWheatGrinded();
            return;
        }
    }

    public void SaveWindmill()
    {
        SaveableWindmill saveableWindmill = new SaveableWindmill();
        saveableWindmill.state = m_currentState;
        saveableWindmill.startingTimestamp = spawnManager.getStartingTimestamp();
        saveableWindmill.currentWheatCount = currentWheatCount;
        saveableWindmill.currentFlourCount = getCurrentFlourCount();

        GameState.Instance.windmill = saveableWindmill;
    }

    int getCurrentFlourCount()
    {
        var snapZone = flourSnapZone.GetComponent<SnapZone>();
        if (snapZone.HeldItem == null)
        {
            return 0;
        }

        return snapZone.HeldItem.GetComponent<ItemStack>().GetStackSize();
    }

    protected void beforeSceneSwitch(object sender, EventArgs e)
    {
        SceneSwitcher.Instance.beforeSceneSwitch -= beforeSceneSwitch;
        SaveWindmill();
    }
}
