using UnityEngine;

public class StartSoundWithRandomOffset : MonoBehaviour
{

    void Start()
    {
        var audio = GetComponent<AudioSource>();
        audio.time = Random.Range(0, 3f);
    }
}
