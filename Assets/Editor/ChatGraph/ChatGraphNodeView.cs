using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace OpenHarvest.EditorTools.ChatGraph
{
    internal sealed class ChatGraphNodeView : Node
    {
        public readonly ChatTreeNodeData Data;
        public Port InputPort;
        public readonly List<Port> OutputPorts = new List<Port>();

        public ChatGraphNodeView(ChatTreeNodeData data)
        {
            Data = data;
            title = string.IsNullOrEmpty(data.Id) ? "(no id)" : data.Id;
            style.width = 420f;
            style.maxWidth = 420f;

            InputPort = Port.Create<Edge>(
                Orientation.Horizontal,
                Direction.Input,
                Port.Capacity.Multi,
                typeof(bool));
            InputPort.portName = "In";
            inputContainer.Add(InputPort);

            var bodyContainer = new VisualElement();
            bodyContainer.style.flexDirection = FlexDirection.Column;
            bodyContainer.style.marginTop = 6f;
            bodyContainer.style.maxWidth = 390f;

            var bodyTitle = new Label("Body");
            bodyTitle.style.unityFontStyleAndWeight = FontStyle.Bold;
            bodyTitle.style.marginBottom = 2f;
            bodyContainer.Add(bodyTitle);

            var bodyLabel = new Label(data.body ?? string.Empty);
            bodyLabel.style.whiteSpace = WhiteSpace.Normal;
            bodyLabel.style.maxWidth = 390f;
            bodyLabel.style.flexShrink = 1f;
            bodyContainer.Add(bodyLabel);

            extensionContainer.Add(bodyContainer);

            if (data.choices == null)
            {
                return;
            }

            for (int i = 0; i < data.choices.Count; i++)
            {
                ChatChoiceData ch = data.choices[i];
                Port p = Port.Create<Edge>(
                    Orientation.Horizontal,
                    Direction.Output,
                    Port.Capacity.Single,
                    typeof(bool));
                p.portName = ch == null || string.IsNullOrEmpty(ch.label)
                    ? $"Out {i + 1}"
                    : ch.label;
                p.userData = new OutputPortKey
                {
                    SourceNodeId = data.Id,
                    ChoiceIndex = i,
                };
                outputContainer.Add(p);
                OutputPorts.Add(p);
            }

            RefreshExpandedState();
            RefreshPorts();
        }
    }
}
