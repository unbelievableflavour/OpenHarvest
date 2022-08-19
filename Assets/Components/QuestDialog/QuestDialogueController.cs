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
 
public class QuestDialogueController : MonoBehaviour
{
    [Header("General settings")]
    public Dialogue dialogue;
    public TalkUIController talkUIController;

    [Header("Quest specific")]
    public Quests questId;
    public string NPCName;
    public QuestDialogue[] dialogues;

    private int currentDialogue = 0;

    void Start()
    {
        currentDialogue = GameState.Instance.questList[questId].currentDialogue;
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
    }
}
