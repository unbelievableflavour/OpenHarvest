using UnityEngine;

public class GeneralQuestController : MonoBehaviour
{
    public static GeneralQuestController Instance = null;

    private AudioSource audioSource;
    public AudioClip soundOnQuestStarted;
    public AudioClip soundOnQuestUpdated;
    public AudioClip soundOnQuestFinished;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayQuestStartedSound()
    {
        playSound(soundOnQuestStarted);
    }

    public void PlayQuestUpdatedSound()
    {
        playSound(soundOnQuestUpdated);
    }

    public void PlayQuestFinishedSound()
    {
        playSound(soundOnQuestFinished);
    }

    private void playSound(AudioClip clip)
    {
        if (audioSource)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
