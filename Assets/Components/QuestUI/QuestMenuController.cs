using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenuController : MonoBehaviour
{
    public ViewSwitcher viewSwitcher;
    public GameObject QuestRow;
    public Transform QuestList;

    public Text detailsHeader;
    public Text detailsTipValue;

    private string view = "list";
    private int currentQuestIndex;

    private void OnEnable()
    {
        ActivateUI();
    }

    public void ActivateUI()
    {
        view = "list";
        RefreshView();
    }

    public void RefreshView()
    {
        if (viewSwitcher != null && viewSwitcher.views != null && viewSwitcher.views.Count > 0)
        {
            viewSwitcher.setActiveView(view);
        }

        if (view == "list")
        {
            fillQuestsList();
            return;
        }

        if (view != "details")
        {
            return;
        }

        ApplyDetailsTexts();
    }

    public void SetDetailView(int questIndex)
    {
        currentQuestIndex = questIndex;
        view = "details";
        RefreshView();
    }

    public void ShowQuestListView()
    {
        view = "list";
        RefreshView();
    }

    private void ApplyDetailsTexts()
    {
        if (QuestRuntimeService.Instance == null)
        {
            return;
        }

        List<QuestRuntimeMenuEntry> entries = QuestRuntimeService.Instance.GetQuestMenuEntries();
        if (currentQuestIndex < 0 || currentQuestIndex >= entries.Count)
        {
            if (detailsHeader != null)
            {
                detailsHeader.text = "Quest";
            }

            if (detailsTipValue != null)
            {
                detailsTipValue.text = string.Empty;
            }

            return;
        }

        QuestRuntimeMenuEntry entry = entries[currentQuestIndex];
        if (detailsHeader != null)
        {
            detailsHeader.text = entry.displayName;
        }

        if (detailsTipValue != null)
        {
            if (entry.currentStatus == QuestMenuProgressStatus.Completed)
            {
                detailsTipValue.text = "This quest is complete.";
                return;
            }

            string hint = QuestRuntimeService.Instance.GetQuestJournalDetailHint(entry.questId);
            if (string.IsNullOrWhiteSpace(hint))
            {
                detailsTipValue.text = "No journal hint for this step.";
                return;
            }

            detailsTipValue.text = hint.Trim();
        }
    }

    private void fillQuestsList()
    {
        foreach (Transform child in QuestList)
        {
            Destroy(child.gameObject);
        }

        if (QuestRuntimeService.Instance == null)
        {
            return;
        }

        List<QuestRuntimeMenuEntry> entries = QuestRuntimeService.Instance.GetQuestMenuEntries();
        for (int i = 0; i < entries.Count; i++)
        {
            QuestRuntimeMenuEntry entry = entries[i];
            if (entry == null)
            {
                continue;
            }

            string progress;
            if (entry.currentStatus == QuestMenuProgressStatus.Completed)
            {
                progress = "done";
            }
            else if (entry.currentStatus == QuestMenuProgressStatus.NotStarted)
            {
                progress = "not started";
            }
            else
            {
                progress = "in progress";
            }
            GameObject row = Instantiate(QuestRow);
            row.SetActive(true);
            Text text = row.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = entry.displayName + " (" + progress + ")";
            }

            row.transform.SetParent(QuestList, false);
            QuestRow rowScript = row.GetComponent<QuestRow>();
            if (rowScript != null)
            {
                rowScript.InitialiseQuestRow(this, i);
            }
        }
    }
}
