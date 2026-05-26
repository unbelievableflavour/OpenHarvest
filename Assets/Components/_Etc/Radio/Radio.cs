using UnityEngine;

public class Radio : MonoBehaviour
{
    public AudioSource audio;
    public AudioClip[] myMusic;

    private bool isFirstSong = true;

    void Start()
    {
        playRandomMusic();
    }

    void Update()
    {
        if (!audio.isPlaying)
            playRandomMusic();
    }

    void playRandomMusic()
    {
        audio.clip = myMusic[Random.Range(0, myMusic.Length)] as AudioClip;
        if (isFirstSong)
        {
            audio.time = Random.Range(0, 15f);
        }

        audio.Play();
    }
}
