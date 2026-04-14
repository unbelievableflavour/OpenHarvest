using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSound : MonoBehaviour
{
    public AudioClip[] soundsToPlay;

    public void Play()
    {
        if (soundsToPlay.Length != 0)
        {
            VRUtils.Instance.PlaySpatialClipAt(soundsToPlay[Random.Range(0, soundsToPlay.Length)], transform.position, 1f, 1f);
        }
    }
}
