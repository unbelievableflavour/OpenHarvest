using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Npc Chat Graph",
    menuName = "OpenHarvest/NPC/Chat Graph",
    order = 20)]
public class NpcChatGraph : ScriptableObject
{
    public const int MaxBodyLength = 500;

    public List<ChatTreeNodeData> nodes = new List<ChatTreeNodeData>();

    private void OnValidate()
    {
        EnsureNodeIds();
        ClampBodyLengths();
    }

    public ChatTreeNodeData FindNode(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || nodes == null)
        {
            return null;
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            ChatTreeNodeData n = nodes[i];
            if (n != null && string.Equals(n.Id, id, StringComparison.Ordinal))
            {
                return n;
            }
        }

        return null;
    }

    public bool TryValidate(out string errorMessage)
    {
        if (nodes == null || nodes.Count == 0)
        {
            errorMessage = "No nodes defined.";
            return false;
        }

        var idSet = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < nodes.Count; i++)
        {
            ChatTreeNodeData n = nodes[i];
            if (n == null)
            {
                errorMessage = $"Node at index {i} is null.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(n.Id))
            {
                errorMessage = $"Node at index {i} has an empty id.";
                return false;
            }

            if (!idSet.Add(n.Id))
            {
                errorMessage = $"Duplicate node id '{n.Id}'.";
                return false;
            }
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            ChatTreeNodeData n = nodes[i];
            if (n.choices == null)
            {
                continue;
            }

            for (int c = 0; c < n.choices.Count; c++)
            {
                ChatChoiceData ch = n.choices[c];
                if (ch == null)
                {
                    errorMessage = $"Node '{n.Id}' has a null choice.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(ch.nextNodeId))
                {
                    continue;
                }

                if (FindNode(ch.nextNodeId) == null)
                {
                    errorMessage = $"Node '{n.Id}' choice '{ch.label}' points to missing node '{ch.nextNodeId}'.";
                    return false;
                }
            }
        }

        errorMessage = null;
        return true;
    }

    public ChatTreeNodeData GetEntryNode()
    {
        if (nodes == null || nodes.Count == 0)
        {
            return null;
        }

        return nodes[0];
    }

    public void EnsureNodeIds()
    {
        if (nodes == null)
        {
            return;
        }

        var used = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < nodes.Count; i++)
        {
            ChatTreeNodeData node = nodes[i];
            if (node == null)
            {
                continue;
            }

            string current = node.Id;
            if (string.IsNullOrWhiteSpace(current) || used.Contains(current))
            {
                current = "node_" + Guid.NewGuid().ToString("N").Substring(0, 8);
                node.SetId(current);
            }

            used.Add(current);
        }
    }

    public void ClampBodyLengths()
    {
        if (nodes == null)
        {
            return;
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            ChatTreeNodeData node = nodes[i];
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
}
