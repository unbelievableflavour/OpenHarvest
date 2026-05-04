using System;

public sealed class NpcChatSession
{
    private readonly NpcChatGraph _graph;
    private string _activeNodeId;

    public NpcChatSession(NpcChatGraph graph)
    {
        _graph = graph;
        ChatTreeNodeData entry = graph != null ? graph.GetEntryNode() : null;
        _activeNodeId = entry != null ? entry.Id : null;
    }

    public string ActiveNodeId => _activeNodeId;

    public bool IsFinished => string.IsNullOrEmpty(_activeNodeId);

    public bool TryGetCurrentNode(out ChatTreeNodeData node)
    {
        node = null;
        if (_graph == null || string.IsNullOrEmpty(_activeNodeId))
        {
            return false;
        }

        node = _graph.FindNode(_activeNodeId);
        return node != null;
    }

    public bool TryChoose(int choiceIndex, out bool chatEnded)
    {
        chatEnded = false;
        if (!TryGetCurrentNode(out ChatTreeNodeData current))
        {
            chatEnded = true;
            return false;
        }

        if (current.choices == null || current.choices.Count == 0)
        {
            _activeNodeId = null;
            chatEnded = true;
            return true;
        }

        if (choiceIndex < 0 || choiceIndex >= current.choices.Count)
        {
            return false;
        }

        ChatChoiceData choice = current.choices[choiceIndex];
        if (choice == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(choice.nextNodeId))
        {
            _activeNodeId = null;
            chatEnded = true;
            return true;
        }

        ChatTreeNodeData next = _graph.FindNode(choice.nextNodeId);
        if (next == null)
        {
            _activeNodeId = null;
            chatEnded = true;
            return true;
        }

        _activeNodeId = next.Id;
        chatEnded = false;
        return true;
    }
}
