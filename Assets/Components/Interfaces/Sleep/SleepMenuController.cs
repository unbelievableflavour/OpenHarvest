using System.Collections;
using UnityEngine;

public class SleepMenuController : MonoBehaviour
{
    public SceneSwitcher sceneSwitcher;
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
        sceneSwitcher.Sleep();
        StartCoroutine(awaitFadeDuration());
    }

    IEnumerator awaitFadeDuration()
    {
        yield return new WaitForSeconds(sceneSwitcher.playerInvokes.screen.FadeOutSpeed);
        TimeController.Sleep();
        mainCanvas.SetActive(false);
        sleepSuccessfulCanvas.SetActive(true);
    }
}
