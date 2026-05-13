using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Quest row progress for the journal / quest menu list.</summary>
public enum QuestMenuProgressStatus
{
    NotStarted,
    InProgress,
    Completed,
}

[Serializable]
public class QuestRuntimeMenuEntry
{
    public string questId;
    public string displayName;
    public QuestMenuProgressStatus currentStatus;
}

public class QuestRuntimeService : MonoBehaviour
{
    private sealed class QuestState
    {
        public QuestGraph Graph;
        public QuestNodeBase CurrentNode;
        public QuestGiftNode PendingGiftNode;
        public bool IsCompleted;
    }

    private readonly List<QuestState> _states = new List<QuestState>();

    public static QuestRuntimeService Instance { get; private set; }

    public event Action<NpcProximityInteractable> OnGenericGiftRequested;

    /// <summary>
    /// Raised after V2 quest progress is written to <see cref="GameState.questRuntimeStates"/> (trimmed quest id).
    /// </summary>
    public event Action<string> QuestRuntimeStateChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFromDatabase();
            return;
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public List<QuestNodeBase> GetVisibleNodesForNpc(NpcProximityInteractable interactable)
    {
        var nodes = new List<QuestNodeBase>();
        if (interactable == null)
        {
            return nodes;
        }

        for (int i = 0; i < _states.Count; i++)
        {
            QuestState state = _states[i];
            if (state == null || state.IsCompleted || state.CurrentNode == null)
            {
                continue;
            }

            if (state.CurrentNode is QuestWorldObjectiveNode)
            {
                continue;
            }

            if (!state.CurrentNode.CanRenderFor(interactable))
            {
                continue;
            }

            nodes.Add(state.CurrentNode);
        }

        return nodes;
    }

    public List<QuestRuntimeMenuEntry> GetQuestMenuEntries()
    {
        var entries = new List<QuestRuntimeMenuEntry>();
        QuestDatabase db = DatabaseManager.Instance != null ? DatabaseManager.Instance.quests : null;
        if (db == null || db.quests == null)
        {
            return entries;
        }

        Dictionary<string, QuestState> stateByQuestId = BuildQuestStateLookupByQuestId();

        for (int i = 0; i < db.quests.Count; i++)
        {
            QuestGraph graph = db.quests[i];
            if (graph == null || graph.GetEntryNode() == null)
            {
                continue;
            }

            string questId = string.IsNullOrWhiteSpace(graph.questId) ? string.Empty : graph.questId.Trim();
            string displayName = string.IsNullOrWhiteSpace(graph.displayName) ? "Quest" : graph.displayName.Trim();

            stateByQuestId.TryGetValue(questId, out QuestState state);

            QuestMenuProgressStatus resolvedStatus = ResolveMenuProgressStatus(state, graph, questId);

            entries.Add(new QuestRuntimeMenuEntry
            {
                questId = questId,
                displayName = displayName,
                currentStatus = resolvedStatus
            });
        }

        return entries;
    }

    private static QuestMenuProgressStatus ResolveMenuProgressStatus(QuestState state, QuestGraph graph, string questId)
    {
        if (state != null)
        {
            if (state.IsCompleted)
            {
                return QuestMenuProgressStatus.Completed;
            }

            if (IsAtQuestEntry(state))
            {
                return QuestMenuProgressStatus.NotStarted;
            }

            return QuestMenuProgressStatus.InProgress;
        }

        if (TryGetQuestCompletedFromSave(questId, out bool completedFromSave) && completedFromSave)
        {
            return QuestMenuProgressStatus.Completed;
        }

        if (IsSavedProgressAtEntryOrUnknown(graph, questId))
        {
            return QuestMenuProgressStatus.NotStarted;
        }

        return QuestMenuProgressStatus.InProgress;
    }

    /// <summary>
    /// Journal hint for the quest detail UI: first tip along the main path when not yet started,
    /// otherwise the current chat/gift step tip. Empty when complete or when no tip is set.
    /// </summary>
    public string GetQuestJournalDetailHint(string questId)
    {
        if (string.IsNullOrWhiteSpace(questId))
        {
            return string.Empty;
        }

        string qid = questId.Trim();
        QuestGraph graph = FindQuestGraphByQuestId(qid);
        if (graph == null)
        {
            return string.Empty;
        }

        QuestState state = FindQuestStateByQuestId(qid);
        if (state != null)
        {
            if (state.IsCompleted)
            {
                return string.Empty;
            }

            if (IsAtQuestEntry(state))
            {
                return GetFirstJournalTipAlongMainPath(graph);
            }

            return GetMenuTipForState(state);
        }

        if (TryGetQuestCompletedFromSave(qid, out bool completedFromSave) && completedFromSave)
        {
            return string.Empty;
        }

        if (IsSavedProgressAtEntryOrUnknown(graph, qid))
        {
            return GetFirstJournalTipAlongMainPath(graph);
        }

        return string.Empty;
    }

    private static QuestGraph FindQuestGraphByQuestId(string questId)
    {
        if (string.IsNullOrWhiteSpace(questId))
        {
            return null;
        }

        QuestDatabase db = DatabaseManager.Instance != null ? DatabaseManager.Instance.quests : null;
        if (db == null)
        {
            return null;
        }

        return db.FindById(questId.Trim());
    }

    private QuestState FindQuestStateByQuestId(string questId)
    {
        if (string.IsNullOrWhiteSpace(questId))
        {
            return null;
        }

        string qid = questId.Trim();
        for (int i = 0; i < _states.Count; i++)
        {
            QuestState state = _states[i];
            if (state?.Graph == null || string.IsNullOrWhiteSpace(state.Graph.questId))
            {
                continue;
            }

            if (string.Equals(state.Graph.questId.Trim(), qid, StringComparison.Ordinal))
            {
                return state;
            }
        }

        return null;
    }

    private Dictionary<string, QuestState> BuildQuestStateLookupByQuestId()
    {
        var map = new Dictionary<string, QuestState>(StringComparer.Ordinal);
        for (int i = 0; i < _states.Count; i++)
        {
            QuestState state = _states[i];
            if (state?.Graph == null || string.IsNullOrWhiteSpace(state.Graph.questId))
            {
                continue;
            }

            map[state.Graph.questId.Trim()] = state;
        }

        return map;
    }

    private static bool TryGetQuestCompletedFromSave(string questId, out bool isCompleted)
    {
        isCompleted = false;
        if (string.IsNullOrWhiteSpace(questId) || GameState.Instance == null || GameState.Instance.questRuntimeStates == null)
        {
            return false;
        }

        if (!GameState.Instance.questRuntimeStates.TryGetValue(questId, out QuestRuntimeProgressState saved))
        {
            return false;
        }

        isCompleted = saved.isCompleted;
        return true;
    }

    private static bool IsSavedProgressAtEntryOrUnknown(QuestGraph graph, string questId)
    {
        if (graph == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(questId) || GameState.Instance == null || GameState.Instance.questRuntimeStates == null)
        {
            return true;
        }

        if (!GameState.Instance.questRuntimeStates.TryGetValue(questId, out QuestRuntimeProgressState saved))
        {
            return true;
        }

        return IsSavedProgressAtEntry(graph, saved);
    }

    private static bool IsSavedProgressAtEntry(QuestGraph graph, QuestRuntimeProgressState saved)
    {
        if (graph == null || saved == null || saved.isCompleted)
        {
            return false;
        }

        if (saved.pendingGiftNodeIndex >= 0)
        {
            return false;
        }

        int entryIdx = GetNodeIndex(graph, graph.GetEntryNode());
        if (entryIdx < 0)
        {
            return false;
        }

        if (saved.currentNodeIndex < 0)
        {
            return true;
        }

        return saved.currentNodeIndex == entryIdx;
    }

    private static bool IsAtQuestEntry(QuestState state)
    {
        if (state == null || state.IsCompleted || state.Graph == null)
        {
            return false;
        }

        if (state.PendingGiftNode != null)
        {
            return false;
        }

        int entryIdx = GetNodeIndex(state.Graph, state.Graph.GetEntryNode());
        int currentIdx = GetNodeIndex(state.Graph, state.CurrentNode);
        if (entryIdx < 0 || currentIdx < 0)
        {
            return false;
        }

        return currentIdx == entryIdx;
    }

    private static string GetFirstJournalTipAlongMainPath(QuestGraph graph)
    {
        if (graph == null)
        {
            return string.Empty;
        }

        QuestNodeBase node = graph.GetEntryNode();
        const int maxSteps = 256;
        for (int step = 0; step < maxSteps && node != null; step++)
        {
            if (node is QuestGiftNode giftNode)
            {
                if (!string.IsNullOrWhiteSpace(giftNode.tip))
                {
                    return giftNode.tip.Trim();
                }
            }
            else if (node is QuestChatNode chatNode)
            {
                if (!string.IsNullOrWhiteSpace(chatNode.tip))
                {
                    return chatNode.tip.Trim();
                }
            }
            else if (node is QuestWorldObjectiveNode worldNode)
            {
                if (!string.IsNullOrWhiteSpace(worldNode.tip))
                {
                    return worldNode.tip.Trim();
                }
            }

            node = node.GetNextNode();
        }

        return string.Empty;
    }

    private static string GetMenuTipForState(QuestState state)
    {
        if (state == null || state.IsCompleted)
        {
            return string.Empty;
        }

        if (state.CurrentNode is QuestGiftNode giftNode)
        {
            return string.IsNullOrWhiteSpace(giftNode.tip) ? string.Empty : giftNode.tip.Trim();
        }

        if (state.CurrentNode is QuestChatNode chatNode)
        {
            return string.IsNullOrWhiteSpace(chatNode.tip) ? string.Empty : chatNode.tip.Trim();
        }

        if (state.CurrentNode is QuestWorldObjectiveNode worldNode)
        {
            return string.IsNullOrWhiteSpace(worldNode.tip) ? string.Empty : worldNode.tip.Trim();
        }

        return string.Empty;
    }

    public void RunNodeAction(QuestNodeBase node, InteractionUIController interactionUI, NpcProximityInteractable interactable)
    {
        if (node == null)
        {
            return;
        }

        QuestState state = FindStateByNode(node);
        if (state == null || state.IsCompleted)
        {
            return;
        }

        if (node is QuestGiftNode giftNode)
        {
            state.PendingGiftNode = giftNode;
            SaveState(state);
            giftNode.RunAction(interactionUI, interactable, state.Graph);
            PromptNpcGiftHandoff(interactable);
            OnGenericGiftRequested?.Invoke(interactable);
            return;
        }

        if (node is QuestWorldObjectiveNode worldObjectiveNode)
        {
            worldObjectiveNode.RunAction(interactionUI, interactable, state.Graph);
            return;
        }

        if (node is QuestChatNode)
        {
            node.RunAction(interactionUI, interactable, state.Graph);
            return;
        }

        node.RunAction(interactionUI, interactable, state.Graph);
        AdvanceState(state, node);

        if (node is QuestFinishNode)
        {
            EventManager.Emit("interactionUIToMain");
        }
    }

    public bool TrySubmitGift(
        NpcProximityInteractable interactable,
        string itemId,
        int handedAmount,
        out int requiredAmount)
    {
        requiredAmount = 0;
        if (interactable == null)
        {
            return false;
        }

        for (int i = 0; i < _states.Count; i++)
        {
            QuestState state = _states[i];
            if (state == null || state.IsCompleted || state.PendingGiftNode == null)
            {
                continue;
            }

            if (!state.PendingGiftNode.CanRenderFor(interactable))
            {
                continue;
            }

            if (!state.PendingGiftNode.IsGiftMatch(itemId))
            {
                continue;
            }

            int needed = state.PendingGiftNode.GetRequiredAmount();
            if (handedAmount < needed)
            {
                return false;
            }

            QuestGiftNode giftedNode = state.PendingGiftNode;
            state.PendingGiftNode = null;
            requiredAmount = needed;
            AdvanceState(state, giftedNode);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Advances past a <see cref="QuestWorldObjectiveNode"/> when external gameplay (e.g. tree hang count) is satisfied.
    /// </summary>
    public bool TryCompleteWorldObjective(string questId, string objectiveKey)
    {
        if (string.IsNullOrWhiteSpace(questId) || string.IsNullOrWhiteSpace(objectiveKey))
        {
            return false;
        }

        string qid = questId.Trim();
        string ok = objectiveKey.Trim();

        for (int i = 0; i < _states.Count; i++)
        {
            QuestState state = _states[i];
            if (state == null || state.IsCompleted || state.Graph == null || string.IsNullOrWhiteSpace(state.Graph.questId))
            {
                continue;
            }

            if (!string.Equals(state.Graph.questId.Trim(), qid, StringComparison.Ordinal))
            {
                continue;
            }

            if (!(state.CurrentNode is QuestWorldObjectiveNode worldNode))
            {
                continue;
            }

            if (!string.Equals(worldNode.ObjectiveKey, ok, StringComparison.Ordinal))
            {
                continue;
            }

            AdvanceState(state, worldNode);
            return true;
        }

        return false;
    }

    public void RequestGenericGift(NpcProximityInteractable interactable)
    {
        PromptNpcGiftHandoff(interactable);
        OnGenericGiftRequested?.Invoke(interactable);
    }

    public bool RunCurrentNodeForNpc(NpcProximityInteractable interactable, InteractionUIController interactionUI)
    {
        if (interactable == null)
        {
            return false;
        }

        List<QuestNodeBase> nodes = GetVisibleNodesForNpc(interactable);
        if (nodes.Count == 0 || nodes[0] == null)
        {
            return false;
        }

        RunNodeAction(nodes[0], interactionUI, interactable);
        return true;
    }

    public bool ContinueQuestChatForNode(QuestNodeBase node, NpcProximityInteractable interactable, InteractionUIController interactionUI)
    {
        if (node == null)
        {
            return false;
        }

        QuestState state = FindStateByNode(node);
        if (state == null || state.IsCompleted || state.CurrentNode == null || state.CurrentNode != node)
        {
            return false;
        }

        AdvanceState(state, state.CurrentNode);
        if (state.IsCompleted || state.CurrentNode == null)
        {
            return false;
        }

        if (!state.CurrentNode.CanRenderFor(interactable))
        {
            return false;
        }

        RunNodeAction(state.CurrentNode, interactionUI, interactable);
        return true;
    }

    private static void PromptNpcGiftHandoff(NpcProximityInteractable interactable)
    {
        if (interactable == null)
        {
            return;
        }

        NPCController npc = interactable.GetComponentInParent<NPCController>();
        if (npc == null)
        {
            npc = interactable.GetComponent<NPCController>();
        }

        if (npc == null)
        {
            return;
        }

        npc.HoldOutHand();
    }

    private void InitializeFromDatabase()
    {
        _states.Clear();
        QuestDatabase db = DatabaseManager.Instance != null ? DatabaseManager.Instance.quests : null;
        if (db == null || db.quests == null)
        {
            return;
        }

        for (int i = 0; i < db.quests.Count; i++)
        {
            QuestGraph graph = db.quests[i];
            if (graph == null || graph.GetEntryNode() == null)
            {
                continue;
            }

            QuestRuntimeProgressState saved = null;
            if (GameState.Instance != null && GameState.Instance.questRuntimeStates != null && !string.IsNullOrWhiteSpace(graph.questId))
            {
                GameState.Instance.questRuntimeStates.TryGetValue(graph.questId, out saved);
            }

            var state = new QuestState
            {
                Graph = graph,
                CurrentNode = saved != null ? GetNodeAtIndex(graph, saved.currentNodeIndex) : graph.GetEntryNode(),
                PendingGiftNode = saved != null ? GetNodeAtIndex(graph, saved.pendingGiftNodeIndex) as QuestGiftNode : null,
                IsCompleted = saved != null && saved.isCompleted,
            };

            if (!state.IsCompleted && state.CurrentNode == null)
            {
                state.CurrentNode = graph.GetEntryNode();
            }

            if (saved == null)
            {
                PlayQuestStartedSound();
            }

            _states.Add(state);
            SaveState(state);
        }
    }

    private void AdvanceState(QuestState state, QuestNodeBase fromNode)
    {
        if (state == null || fromNode == null)
        {
            return;
        }

        if (fromNode is QuestFinishNode)
        {
            state.IsCompleted = true;
            state.CurrentNode = null;
            state.PendingGiftNode = null;
            PlayQuestFinishedSound();
            SaveState(state);
            return;
        }

        QuestNodeBase next = fromNode.GetNextNode();
        if (next == null)
        {
            state.IsCompleted = true;
            state.CurrentNode = null;
            state.PendingGiftNode = null;
            PlayQuestFinishedSound();
            SaveState(state);
            return;
        }

        state.CurrentNode = next;
        state.PendingGiftNode = null;
        PlayQuestUpdatedSound();
        SaveState(state);
    }

    private static QuestNodeBase GetNodeAtIndex(QuestGraph graph, int nodeIndex)
    {
        if (graph == null || graph.nodes == null || nodeIndex < 0 || nodeIndex >= graph.nodes.Count)
        {
            return null;
        }

        return graph.nodes[nodeIndex] as QuestNodeBase;
    }

    private static int GetNodeIndex(QuestGraph graph, QuestNodeBase node)
    {
        if (graph == null || graph.nodes == null || node == null)
        {
            return -1;
        }

        for (int i = 0; i < graph.nodes.Count; i++)
        {
            if (graph.nodes[i] == node)
            {
                return i;
            }
        }

        return -1;
    }

    private void SaveState(QuestState state)
    {
        if (state == null || state.Graph == null || string.IsNullOrWhiteSpace(state.Graph.questId) || GameState.Instance == null)
        {
            return;
        }

        if (GameState.Instance.questRuntimeStates == null)
        {
            GameState.Instance.questRuntimeStates = new Dictionary<string, QuestRuntimeProgressState>();
        }

        GameState.Instance.questRuntimeStates[state.Graph.questId] = new QuestRuntimeProgressState
        {
            currentNodeIndex = GetNodeIndex(state.Graph, state.CurrentNode),
            pendingGiftNodeIndex = GetNodeIndex(state.Graph, state.PendingGiftNode),
            isCompleted = state.IsCompleted
        };

        QuestRuntimeStateChanged?.Invoke(state.Graph.questId);
    }

    private static void PlayQuestStartedSound()
    {
        if (GeneralQuestController.Instance != null)
        {
            GeneralQuestController.Instance.PlayQuestStartedSound();
        }
    }

    private static void PlayQuestUpdatedSound()
    {
        if (GeneralQuestController.Instance != null)
        {
            GeneralQuestController.Instance.PlayQuestUpdatedSound();
        }
    }

    private static void PlayQuestFinishedSound()
    {
        if (GeneralQuestController.Instance != null)
        {
            GeneralQuestController.Instance.PlayQuestFinishedSound();
        }
    }

    private QuestState FindStateByNode(QuestNodeBase node)
    {
        for (int i = 0; i < _states.Count; i++)
        {
            QuestState state = _states[i];
            if (state == null)
            {
                continue;
            }

            if (state.CurrentNode == node || state.PendingGiftNode == node)
            {
                return state;
            }
        }

        return null;
    }
}
