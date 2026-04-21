using UnityEngine;

public class TalkUIOption : MonoBehaviour
{
    [Header("General settings")]
    public GameObject Dialog;

    [Header("Conversation Starter")]
    public string title;
    public Sprite icon;

    protected TalkUIController talkUIController;
    protected NPCController npc;

    public void BackToMenu()
    {
        talkUIController.EnableTalkUI();
    }

    public void SetTalkUIController(TalkUIController talkUIController)
    {
        this.talkUIController = talkUIController;
    }

    public void SetNPCController(NPCController npc)
    {
        this.npc = npc;
    }

    protected void SpeakDialogue(QuestDialogue dialogue)
    {
        if (npc == null || dialogue == null || string.IsNullOrEmpty(dialogue.text))
        {
            return;
        }

        var voice = npc.GetComponent<NPCVoice>();
        if (voice != null)
        {
            voice.Speak(dialogue.text);
        }
    }

    /// <summary>
    /// Called when this option's dialog window is actually opened by the
    /// player (or when the current dialogue advances while the window is
    /// open). Subclasses that have voiced dialogue should override this and
    /// call <see cref="SpeakDialogue"/> with their current line.
    /// Intentionally a no-op by default so silent options (e.g. store) don't
    /// have to implement anything.
    /// </summary>
    public virtual void SpeakCurrentDialogue() { }
}
