using UnityEngine;
using XNode;

public abstract class QuestNodeBase : Node
{
    [Input(ShowBackingValue.Never)] public bool inFlow;
    [Output(ShowBackingValue.Never)] public bool next;

    [Header("Availability")]
    [Tooltip("Only show this quest action on this NPC interaction definition.")]
    public NpcInteractableDefinition targetNpc;

    [Header("Progression")]
    [Tooltip("Mark this node as the end of this quest.")]
    public bool completesQuest;

    public override object GetValue(NodePort port)
    {
        return null;
    }

    public bool CanRenderFor(NpcProximityInteractable interactable)
    {
        if (interactable == null || targetNpc == null)
        {
            return false;
        }

        return interactable.Definition == targetNpc;
    }

    public string GetDisplayLabel()
    {
        QuestGraph questGraph = graph as QuestGraph;
        if (questGraph == null || string.IsNullOrWhiteSpace(questGraph.displayName))
        {
            return "Quest";
        }

        return questGraph.displayName.Trim();
    }

    public QuestNodeBase GetNextNode()
    {
        NodePort outPort = GetOutputPort("next");
        if (outPort == null || !outPort.IsConnected)
        {
            return null;
        }

        return outPort.Connection.node as QuestNodeBase;
    }

    public virtual bool IsGiftMatch(string itemId)
    {
        return false;
    }

    public abstract void RunAction(InteractionUIController interactionUI, NpcProximityInteractable interactable, QuestGraph graph);
}
