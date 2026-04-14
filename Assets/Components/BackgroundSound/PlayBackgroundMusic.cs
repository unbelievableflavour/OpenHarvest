using UnityEngine;

public class PlayBackgroundMusic : MonoBehaviour
{
    public AudioClip audioClip;

    void Start()
    {
        AudioManager.Instance.PlayMusic(audioClip);    
    }
}
