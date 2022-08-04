using System;
using UnityEngine;
using static Definitions;

public class SpawnManager : MonoBehaviour
{
    public int daysUntilRespawn = 1;

    public CircleTransitioner timeIndicator;
    public ParticleSystem spawnEffect;
    public GameObject behaviourObject;

    private ObjectSpawner objectSpawner;
    private bool isFed = false;
    private DateTime? startingTimestamp;

    public UnityEngine.Events.UnityEvent afterWaitFunctions;
    public UnityEngine.Events.UnityEvent resetGrowthFunctions;

    void Start()
    {
        TimeController.Instance.ListenToDayChange(handleNewDayStarted);

        objectSpawner = GetComponent<ObjectSpawner>();
        UpdateSpawnManager();
    }

    public void Spawn(bool restartIfAlreadyStarted = false)
    {
        if (restartIfAlreadyStarted){
            timeIndicator.reset();
        }

        isFed = true;
        startingTimestamp = TimeController.getCurrentTime().Date;

        TimeSpan timeLeft = getTimeLeft();
        timeIndicator.doneTime.text = timeLeft.Days.ToString();
        timeIndicator.gameObject.SetActive(true);
    }

    public void resetGrowth()
    {
        if (resetGrowthFunctions != null)
        {
            resetGrowthFunctions.Invoke();
            return;
        }
    }

    public void setStartingTimestamp(DateTime? startingTimestamp)
    {
        this.startingTimestamp = startingTimestamp;
    }

    public DateTime? getStartingTimestamp()
    {
        if (startingTimestamp == null)
        {
            return null;
        }
        return startingTimestamp;
    }

    public void setIsFed(bool isFed)
    {
        this.isFed = isFed;
    }

    public bool getIsFed()
    {
        return isFed;
    }

    public ObjectSpawner getObjectSpawner()
    {
        if (!objectSpawner)
        {
            return null;
        }

        return objectSpawner;
    }

    public void resetGrowthIfLastChild()
    {
        if (!objectSpawner)
        {
            return;
        }

        if (objectSpawner.GetCurrentNumberOfSpawnedObjects() > 0)
        {
            return;
        }
        resetGrowth();
    }

    private void handleNewDayStarted(object sender, EventArgs e)
    {
        UpdateSpawnManager();
    }

    public void UpdateSpawnManager()
    {
        if (!isFed)
        {
            return;
        }

        TimeSpan timeLeft = getTimeLeft();
        timeIndicator.doneTime.text = timeLeft.Days.ToString();

        if (timeLeft.Days > 0)
        {
            return;
        }

        isFed = false;
        timeIndicator.gameObject.SetActive(false);
        spawnEffect.Play();

        if (afterWaitFunctions != null)
        {
            afterWaitFunctions.Invoke();
            return;
        }
    }

    private TimeSpan getTimeLeft()
    {
        var started = (DateTime)getStartingTimestamp();
        var endingDay = started.AddDays(daysUntilRespawn);
        var currentDate = TimeController.getCurrentTime().Date;
        return endingDay - currentDate;
    }
}
