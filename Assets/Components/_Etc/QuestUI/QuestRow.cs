using UnityEngine;

public class QuestRow : MonoBehaviour
{
    private QuestMenuController questMenuController;
    private int questIndex;

    public void InitialiseQuestRow(QuestMenuController questMenuController, int questIndex)
    {
        this.questMenuController = questMenuController;
        this.questIndex = questIndex;
    }

    public void OpenQuestDetails()
    {
        if (questMenuController == null)
        {
            return;
        }

        questMenuController.SetDetailView(questIndex);
    }
}
