using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChatChoiceData
{
    [Tooltip("Button label for this branch.")]
    public string label = "Continue";

    [Tooltip("Id of the next node. Leave empty to end the chat after this line.")]
    public string nextNodeId;
}

[Serializable]
public class ChatTreeNodeData
{
    [SerializeField, HideInInspector]
    private string id;

    public string Id => id;

    [TextArea(3, 10)]
    [Tooltip("Spoken / shown dialogue for this step.")]
    public string body = "";

    [Tooltip("Player choices leaving this node. If empty, show a single Continue that ends the chat.")]
    public List<ChatChoiceData> choices = new List<ChatChoiceData>();

    [HideInInspector]
    public Vector2 graphPosition;

    public void SetId(string newId)
    {
        id = newId;
    }
}
