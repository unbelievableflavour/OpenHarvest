using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestRuntimeMenuEntry
{
    public string questId;
    public string displayName;
    public bool isCompleted;
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
        for (int i = 0; i < _states.Count; i++)
        {
            QuestState state = _states[i];
            if (state == null || state.Graph == null)
            {
                continue;
            }

            entries.Add(new QuestRuntimeMenuEntry
            {
                questId = state.Graph.questId,
                displayName = string.IsNullOrWhiteSpace(state.Graph.displayName) ? "Quest" : state.Graph.displayName.Trim(),
                isCompleted = state.IsCompleted
            });
        }

        return entries;
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

        if (node is QuestChatNode)
        {
            node.RunAction(interactionUI, interactable, state.Graph);
            return;
        }

        node.RunAction(interactionUI, interactable, state.Graph);
        AdvanceState(state, node);
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

    public void RequestGenericGift(NpcProximityInteractable interactable)
    {
        PromptNpcGiftHandoff(interactable);
        OnGenericGiftRequested?.Invoke(interactable);
    }

    public bool ContinueQuestChatForNpc(NpcProximityInteractable interactable, InteractionUIController interactionUI)
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

        QuestNodeBase currentNode = nodes[0];
        QuestState state = FindStateByNode(currentNode);
        if (state == null || state.IsCompleted || state.CurrentNode == null)
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
