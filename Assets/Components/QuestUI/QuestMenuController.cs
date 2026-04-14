using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Definitions;

public class QuestMenuController : MonoBehaviour
{
    public GameObject QuestUI;
    public GameObject QuestRow;
    public Transform QuestList;

    public void ActivateUI()
    {
        RefreshView();        
    }

    public void RefreshView()
    {
        QuestUI.SetActive(true);
        fillQuestsList();
    }

    private void fillQuestsList()
    {
        foreach (Transform child in QuestList)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in GameState.Instance.questList)
        {
            Quest quest = item.Value;
            if(quest.currentProgress == Progress.NotStarted)
            {
                continue;
            }

            var progress = "in progress";
            if(quest.currentProgress == Progress.Done)
            {
                progress = "done";
            }
            GameObject row = Instantiate(QuestRow);
            row.SetActive(true);
            var text = row.GetComponentInChildren<Text>();
            text.text = quest.title + " ("+ progress + ")";
            row.transform.SetParent(QuestList, false);
        }
    }
}