using System;
using UnityEngine;
using static Definitions;
using static UnityEngine.UI.Button;

[System.Serializable]
public class QuestDialogueButton
{
    public string text;
    public ButtonClickedEvent onClick;
}

[System.Serializable]
public class QuestDialogue
{
    [TextArea(3, 10)] [SerializeField] public string text;
    public QuestDialogueButton leftButton;
    public QuestDialogueButton rightButton;
}
 
public class QuestOption : TalkUIOption
{
    [Header("Quest settings")]
    public Quests questId;
    public event Action checkStatus;

    [Header("Dialog settings")]
    public string NPCName;
    public QuestDialogue[] dialogues;

    private int currentDialogue = 0;
    private Dialogue dialogue;

    public void SetTalkDialog(Dialogue dialogue)
    {
        currentDialogue = GameState.Instance.questList[questId].currentDialogue;
        this.dialogue = dialogue;
        UpdateDialogue();
    }

    public void SetCurrentQuestDialog(int newCurrentDialogue)
    {
        currentDialogue = newCurrentDialogue;

        GameState.Instance.questList[questId].currentDialogue = currentDialogue;
        UpdateDialogue();
    }

    public void BackToMenu()
    {
        talkUIController.EnableTalkUI();
    }

    void UpdateDialogue()
    {
        dialogue.UpdateDialogue(NPCName, dialogues[currentDialogue]);
        this.UpdateQuest();
    }

    public TalkUIController getTalkUIController()
    {
        return talkUIController;
    }

    public void StartQuest()
    {
        GeneralQuestController.Instance.StartQuest(questId);
    }

    public void UpdateQuest()
    {
        checkStatus?.Invoke();
    }
}
