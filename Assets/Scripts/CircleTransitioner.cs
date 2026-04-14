using UnityEngine;
using UnityEngine.UI;

public class CircleTransitioner : MonoBehaviour
{
    public Image timeIndicator;
    public Text doneTime;

    private bool indicatorIsRunning = false;
    private bool indicatorIsDone = false;
    private float waitTime;

    void Update()
    {
        if (indicatorIsRunning != true)
        {
            return;
        }

        timeIndicator.fillAmount += 1.0f / waitTime * (Time.deltaTime * (TimeController.isPlayingInRealTime() ? 1 : 60));

        if(timeIndicator.fillAmount == 1.0f)
        {
            indicatorIsDone = true;
        }
    }

    public void setTimer(float newWaitTime)
    {
        timeIndicator.fillAmount = 0;
        waitTime = newWaitTime;
        indicatorIsRunning = true;
    }

    public bool isRunning()
    {
        return indicatorIsRunning;
    }

    public bool isDone()
    {
        return indicatorIsDone;
    }

    public void reset()
    {
        timeIndicator.fillAmount = 0;
        indicatorIsRunning = false;
        indicatorIsDone = false;
    }

    public void DecreaseBySeconds(int secondsThatAlreadyPast)
    {
        timeIndicator.fillAmount = (1.0f / waitTime) * secondsThatAlreadyPast;
    }

    public void SetDoneTime(double numberOfSecondsBeforeFinished)
    {
        doneTime.text = TimeController.getCurrentTime().AddSeconds(numberOfSecondsBeforeFinished).ToString("HH:mm");
    }
}
