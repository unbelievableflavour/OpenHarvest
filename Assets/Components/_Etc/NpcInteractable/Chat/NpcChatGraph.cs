using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(
    fileName = "Npc Chat Graph",
    menuName = "OpenHarvest/NPC/Chat Graph",
    order = 20)]
public class NpcChatGraph : NodeGraph
{
    public const int MaxBodyLength = 500;

    [Tooltip("First node used when a chat starts.")]
    public NpcChatNode entryNode;
    private void OnEnable()
    {
        ClampBodyLengths();
    }

    public bool TryValidate(out string errorMessage)
    {
        if (entryNode == null)
        {
            errorMessage = "Entry node is not assigned.";
            return false;
        }

        List<NpcChatNode> allNodes = GetNodes();
        if (allNodes.Count == 0)
        {
            errorMessage = "No chat nodes found in graph.";
            return false;
        }

        errorMessage = null;
        return true;
    }

    public NpcChatNode GetEntryNode()
    {
        return entryNode;
    }

    public void ClampBodyLengths()
    {
        List<NpcChatNode> allNodes = GetNodes();
        if (allNodes == null)
        {
            return;
        }

        for (int i = 0; i < allNodes.Count; i++)
        {
            NpcChatNode node = allNodes[i];
            if (node == null || string.IsNullOrEmpty(node.body))
            {
                continue;
            }

            if (node.body.Length <= MaxBodyLength)
            {
                continue;
            }

            node.body = node.body.Substring(0, MaxBodyLength);
        }
    }

    private List<NpcChatNode> GetNodes()
    {
        var results = new List<NpcChatNode>();
        if (nodes == null)
        {
            return results;
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] is NpcChatNode node)
            {
                results.Add(node);
            }
        }

        return results;
    }

}
