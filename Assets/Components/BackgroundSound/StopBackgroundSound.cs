using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopBackgroundSound : MonoBehaviour
{
    void Start()
    {
        if (!AudioManager.Instance)
        {
            Debug.Log("audiomanager wasnt found");
            return;
        }
        AudioManager.Instance.StopMusic();
    }
}
