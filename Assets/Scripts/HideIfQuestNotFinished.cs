using System;
using UnityEngine;

/// <summary>
/// Shows this object only when the V2 quest (see <see cref="QuestGraph.questId"/>) is completed; hidden while in progress.
/// </summary>
public class HideIfQuestNotFinished : MonoBehaviour
{
    [Tooltip("Must match the Quest Graph asset's questId (V2).")]
    [SerializeField] private string questId = "";

    private void OnEnable()
    {
        if (QuestRuntimeService.Instance != null)
        {
            QuestRuntimeService.Instance.QuestRuntimeStateChanged += HandleQuestRuntimeStateChanged;
        }

        CheckQuestStatus();
    }

    private void OnDisable()
    {
        if (QuestRuntimeService.Instance != null)
        {
            QuestRuntimeService.Instance.QuestRuntimeStateChanged -= HandleQuestRuntimeStateChanged;
        }
    }

    public void CheckQuestStatus()
    {
        if (GameState.Instance == null)
        {
            return;
        }

        if (!TryGetQuestIdKey(out string key))
        {
            gameObject.SetActive(false);
            return;
        }

        if (GameState.Instance.questRuntimeStates == null ||
            !GameState.Instance.questRuntimeStates.TryGetValue(key, out QuestRuntimeProgressState state))
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(state.isCompleted);
    }

    private void HandleQuestRuntimeStateChanged(string changedQuestId)
    {
        if (!TryGetQuestIdKey(out string key))
        {
            return;
        }

        string incoming = (changedQuestId ?? string.Empty).Trim();
        if (!string.Equals(incoming, key, StringComparison.Ordinal))
        {
            return;
        }

        CheckQuestStatus();
    }

    private bool TryGetQuestIdKey(out string key)
    {
        if (string.IsNullOrWhiteSpace(questId))
        {
            key = null;
            return false;
        }

        key = questId.Trim();
        return true;
    }
}
