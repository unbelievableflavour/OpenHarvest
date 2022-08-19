using System;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance = null;

    private static bool useRealTime = false; //default = false
    private static int simulatedTimeIncreasePerSecond = 45;
    private float timer;
    private static DateTime currentTime;
    private static double totalSimulatedGameSeconds;
    private static double totalSecondsSpendIngame;

    private static double startingTimeOffset = 28800; //Game should start at 8 o clock. So skip 8 hours.

    private static DateTime? timeBeforeLastScene; //Game should add time between scenes after loading new scene.

    private event EventHandler newDayStarted;

    // Initialize instance.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        SceneSwitcher.Instance.beforeSceneSwitch += beforeSceneSwitch;
        if (timeBeforeLastScene != null)
        {
            AddTimeBetweenScenesToCounters();
        }

        if (totalSimulatedGameSeconds != 0) {
            return;
        }

        totalSimulatedGameSeconds = startingTimeOffset;
        currentTime = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer < 1)
        {
            return;
        }
        timer = 0;

        currentTime = DateTime.Now;
        totalSimulatedGameSeconds += simulatedTimeIncreasePerSecond;
        totalSecondsSpendIngame += 1;
        CheckIfNewDay();
    }

    public void CheckIfNewDay()
    {
        int currentDay = (int)GetTotalSimulatedGameSeconds() / 86400;
        if (currentDay != GameState.Instance.currentDay)
        {
            GameState.Instance.currentDay = currentDay;
            newDayStarted?.Invoke(this, null);
        }
    }

    public static DateTime getCurrentTime()
    {
        if (useRealTime)
        {
            return currentTime;
        }

        return new DateTime().AddSeconds(totalSimulatedGameSeconds);
    }

    public static bool isPlayingInRealTime()
    {
        return useRealTime;
    }

    public static void SetPlayInRealTime()
    {
        useRealTime = true;
    }

    public static void SetPlayInSimulatedTime()
    {
        useRealTime = false;
    }

    public static bool isDay()
    {
        return getCurrentTime().Hour >= 6 && getCurrentTime().Hour <= 20;
    }

    public static bool isNight()
    {
        return getCurrentTime().Hour < 6 || getCurrentTime().Hour > 20;
    }

    public static double GetTotalSecondsSpentIngame()
    {
        return totalSecondsSpendIngame;
    }

    public static void SetTotalSecondsSpentIngame(double seconds)
    {
        totalSecondsSpendIngame = seconds;
    }

    public static double GetTotalSimulatedGameSeconds()
    {
        return totalSimulatedGameSeconds;
    }

    public static void SetTotalSimulatedGameSeconds(double seconds)
    {
        totalSimulatedGameSeconds = seconds;
    }

    public static void Sleep()
    {
        DateTime now = getCurrentTime();
        DateTime tomorrow = now.AddDays(1);
        TimeSpan duration = tomorrow.Date - now;

        totalSimulatedGameSeconds += (duration.TotalSeconds + 28800); //Sleep till next day + add 8 hours
    }

    public static int getSimulatedTimeIncreasePerSecond()
    {
        return simulatedTimeIncreasePerSecond;
    }

    public void SaveTimeBeforeSceneSwitch()
    {
        timeBeforeLastScene = DateTime.Now;
    }

    void AddTimeBetweenScenesToCounters()
    {
        TimeSpan timeSinceWatering = DateTime.Now - (DateTime)timeBeforeLastScene;

        totalSimulatedGameSeconds += timeSinceWatering.TotalSeconds * simulatedTimeIncreasePerSecond;
        totalSecondsSpendIngame += timeSinceWatering.TotalSeconds;

        timeBeforeLastScene = null;
    }

    public void ListenToDayChange(EventHandler newFunction)
    {
        Instance.newDayStarted += newFunction;
    }

    public void RemoveFromDayChange(EventHandler newFunction)
    {
        Instance.newDayStarted -= newFunction;
    }

    protected void beforeSceneSwitch(object sender, EventArgs e)
    {
        SceneSwitcher.Instance.beforeSceneSwitch -= beforeSceneSwitch;
        SaveTimeBeforeSceneSwitch();
    }
}
