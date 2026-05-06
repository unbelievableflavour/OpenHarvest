using System;
using System.Collections.Generic;
using UnityEngine;

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
            OnGenericGiftRequested?.Invoke(interactable);
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
        OnGenericGiftRequested?.Invoke(interactable);
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

            _states.Add(new QuestState
            {
                Graph = graph,
                CurrentNode = graph.GetEntryNode(),
                PendingGiftNode = null,
                IsCompleted = false,
            });
        }
    }

    private static void AdvanceState(QuestState state, QuestNodeBase fromNode)
    {
        if (state == null || fromNode == null)
        {
            return;
        }

        if (fromNode.completesQuest)
        {
            state.IsCompleted = true;
            state.CurrentNode = null;
            return;
        }

        QuestNodeBase next = fromNode.GetNextNode();
        if (next == null)
        {
            state.IsCompleted = true;
            state.CurrentNode = null;
            return;
        }

        state.CurrentNode = next;
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
