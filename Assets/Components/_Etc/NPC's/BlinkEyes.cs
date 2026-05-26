using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkEyes : MonoBehaviour
{
    public GameObject eyesOpen;
    public GameObject eyesClosed;

    //Time before first blink
    float Timer = 3;
    float BlinkTime = 0.2f;

    void Update()
    {
        Timer -= Time.deltaTime;
        if (Timer <= 0f)
        {
            StartCoroutine(Blink());
        }
    }

    IEnumerator Blink()
    {
        eyesClosed.SetActive(true);
        eyesOpen.SetActive(false);
        yield return new WaitForSeconds(BlinkTime);
        eyesClosed.SetActive(false);
        eyesOpen.SetActive(true);
        Timer = Random.Range(0.4f, 3.0f);
    }
}
