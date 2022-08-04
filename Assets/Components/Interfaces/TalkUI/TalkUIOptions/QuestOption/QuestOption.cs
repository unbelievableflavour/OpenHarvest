using UnityEngine;
using static Definitions;

public class QuestOption : TalkUIOption
{
    [Header("Quest settings")]
    public Quests questId;

    public void StartQuest()
    {
        GeneralQuestController.Instance.StartQuest(questId);
    }
}
