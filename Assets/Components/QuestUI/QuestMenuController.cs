using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenuController : MonoBehaviour
{
    public GameObject QuestRow;
    public Transform QuestList;

    private void OnEnable()
    {
        ActivateUI();
    }

    public void ActivateUI()
    {
        RefreshView();        
    }

    public void RefreshView()
    {
        fillQuestsList();
    }

    private void fillQuestsList()
    {
        foreach (Transform child in QuestList)
        {
            Destroy(child.gameObject);
        }

        if (QuestRuntimeService.Instance != null)
        {
            List<QuestRuntimeMenuEntry> entries = QuestRuntimeService.Instance.GetQuestMenuEntries();
            for (int i = 0; i < entries.Count; i++)
            {
                QuestRuntimeMenuEntry entry = entries[i];
                if (entry == null)
                {
                    continue;
                }

                string progress = entry.isCompleted ? "done" : "in progress";
                GameObject row = Instantiate(QuestRow);
                row.SetActive(true);
                var text = row.GetComponentInChildren<Text>();
                text.text = entry.displayName + " (" + progress + ")";
                row.transform.SetParent(QuestList, false);
            }

            return;
        }

        // Fallback for scenes still running legacy quest flow.
        foreach (var item in GameState.Instance.questList)
        {
            Quest quest = item.Value;
            if (quest.currentProgress == Progress.NotStarted)
            {
                continue;
            }

            string progress = quest.currentProgress == Progress.Done ? "done" : "in progress";
            GameObject row = Instantiate(QuestRow);
            row.SetActive(true);
            var text = row.GetComponentInChildren<Text>();
            text.text = quest.title + " (" + progress + ")";
            row.transform.SetParent(QuestList, false);
        }
    }
}