using System;
using UnityEngine;
using static Definitions;

public class HideIfQuestIsFinished : MonoBehaviour
{
    public Quests questId;

    void Start()
    {
        GeneralQuestController.Instance.questsChanged += handleQuestsChanged;
        CheckQuestStatus();
    }

    public void CheckQuestStatus()
    {
        gameObject.SetActive(GameState.Instance.questList[questId].currentProgress != Progress.Done);
    }

    private void handleQuestsChanged(object sender, EventArgs e)
    {
        CheckQuestStatus();
    }
}
