using UnityEngine;

public class TalkOption : TalkUIOption
{
    [Header("Dialog settings")]
    public string NPCName;
    public QuestDialogue[] dialogues;

    private Dialogue dialogue;
    private int currentDialogue = 0;
    public void SetTalkDialog(Dialogue dialogue)
    {
        this.dialogue = dialogue;
        UpdateDialogue();
    }

    public void UpdateDialogue()
    {
        dialogue.UpdateDialogue(NPCName, dialogues[currentDialogue]);
    }
}