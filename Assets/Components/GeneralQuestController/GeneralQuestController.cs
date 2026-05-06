using System;
using UnityEngine;
using static Definitions;

public class GeneralQuestController : MonoBehaviour
{
    public static GeneralQuestController Instance = null;

    private AudioSource audioSource;
    public AudioClip soundOnQuestStarted;
    public AudioClip soundOnQuestUpdated;
    public AudioClip soundOnQuestFinished;

    public event EventHandler questsChanged;

    // Initialize the singleton instance.
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

    public void StartQuest(Quests questId)
    {
        PlayQuestStartedSound();
        GameState.Instance.questList[questId].currentProgress = Progress.InProgress;
    }

    public void UpdateQuest()
    {
        PlayQuestUpdatedSound();
    }

    public void FinishQuest(Quests questId)
    {
        PlayQuestFinishedSound();
        GameState.Instance.questList[questId].currentProgress = Progress.Done;
        questsChanged?.Invoke(this, null);
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
