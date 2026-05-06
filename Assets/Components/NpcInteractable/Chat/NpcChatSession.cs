public sealed class NpcChatSession
{
    private readonly NpcChatGraph _graph;
    private NpcChatNode _activeNode;

    public NpcChatSession(NpcChatGraph graph)
    {
        _graph = graph;
        _activeNode = graph != null ? graph.GetEntryNode() : null;
    }

    public NpcChatNode ActiveNode => _activeNode;

    public bool IsFinished => _activeNode == null;

    public bool TryGetCurrentNode(out NpcChatNode node)
    {
        node = null;
        if (_graph == null || _activeNode == null)
        {
            return false;
        }

        node = _activeNode;
        return true;
    }

    public bool TryChoose(int choiceIndex, out bool chatEnded)
    {
        chatEnded = false;
        if (!TryGetCurrentNode(out NpcChatNode current))
        {
            chatEnded = true;
            return false;
        }

        if (current.ChoiceCount == 0)
        {
            _activeNode = null;
            chatEnded = true;
            return true;
        }

        if (choiceIndex < 0 || choiceIndex >= current.ChoiceCount)
        {
            return false;
        }

        NpcChatNode next = current.GetNextNode(choiceIndex);
        if (next == null)
        {
            _activeNode = null;
            chatEnded = true;
            return true;
        }

        _activeNode = next;
        chatEnded = false;
        return true;
    }
}
