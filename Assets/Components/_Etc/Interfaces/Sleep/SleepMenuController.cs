using System.Collections;
using UnityEngine;

public class SleepMenuController : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject sleepSuccessfulCanvas;

    void Start()
    {
        if (TimeController.isPlayingInRealTime())
        {
            Destroy(this.gameObject);
        }
    }

    public void Sleep()
    {
        SceneSwitcher.Instance.Sleep();
        StartCoroutine(awaitFadeDuration());
    }

    IEnumerator awaitFadeDuration()
    {
        yield return new WaitForSeconds(SceneSwitcher.Instance.playerInvokes.screen.FadeOutSpeed);
        TimeController.Sleep();
        mainCanvas.SetActive(false);
        sleepSuccessfulCanvas.SetActive(true);
    }
}
