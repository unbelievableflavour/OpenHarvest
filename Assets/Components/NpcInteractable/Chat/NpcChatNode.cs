using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("OpenHarvest/NPC Chat/Chat Node")]
public class NpcChatNode : Node
{
    [Input(ShowBackingValue.Never)] public bool inFlow;

    [TextArea(3, 10)]
    [Tooltip("Spoken / shown dialogue for this step.")]
    public string body = "";

    [Output(dynamicPortList = true)]
    [Tooltip("Each entry becomes an output port and is used as the button label.")]
    public List<string> choices = new List<string>();

    public override object GetValue(NodePort port)
    {
        return null;
    }

    public int ChoiceCount
    {
        get
        {
            return choices != null ? choices.Count : 0;
        }
    }

    public string GetChoiceLabel(int index)
    {
        if (choices == null || index < 0 || index >= choices.Count)
        {
            return "Continue";
        }

        if (string.IsNullOrWhiteSpace(choices[index]))
        {
            return "Continue";
        }

        return choices[index];
    }

    public NpcChatNode GetNextNode(int choiceIndex)
    {
        if (choiceIndex < 0 || choiceIndex >= ChoiceCount)
        {
            return null;
        }

        NodePort port = GetOutputPort($"choices {choiceIndex}");
        if (port == null && choiceIndex == 0)
        {
            // Fallback for graphs/tests where only the base list port exists.
            port = GetOutputPort("choices");
        }

        if (port == null || !port.IsConnected)
        {
            return null;
        }

        return port.Connection.node as NpcChatNode;
    }
}
