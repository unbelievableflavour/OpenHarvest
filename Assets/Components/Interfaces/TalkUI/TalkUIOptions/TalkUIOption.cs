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
}
