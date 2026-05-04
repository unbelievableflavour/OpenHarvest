using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace OpenHarvest.EditorTools.ChatGraph
{
    internal sealed class ChatGraphView : GraphView
    {
        public NpcChatGraph Graph;

        private readonly Dictionary<string, ChatGraphNodeView> _nodeViewsById =
            new Dictionary<string, ChatGraphNodeView>();

        private bool _suppressGraphCallbacks;

        public ChatGraphView()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            graphViewChanged = OnGraphViewChanged;

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatible = new List<Port>();
            if (startPort == null)
            {
                return compatible;
            }

            foreach (Node n in nodes)
            {
                if (n is not ChatGraphNodeView v)
                {
                    continue;
                }

                AddIfCompatible(compatible, startPort, v.InputPort);
                for (int i = 0; i < v.OutputPorts.Count; i++)
                {
                    AddIfCompatible(compatible, startPort, v.OutputPorts[i]);
                }
            }

            return compatible;
        }

        private static void AddIfCompatible(List<Port> list, Port startPort, Port candidate)
        {
            if (candidate == null || startPort == candidate)
            {
                return;
            }

            if (startPort.node == candidate.node)
            {
                return;
            }

            if (startPort.direction == candidate.direction)
            {
                return;
            }

            list.Add(candidate);
        }

        public void RebuildFromGraph()
        {
            _suppressGraphCallbacks = true;
            try
            {
                _nodeViewsById.Clear();
                DeleteElements(new List<GraphElement>(graphElements));

                if (Graph == null || Graph.nodes == null)
                {
                    return;
                }

                for (int i = 0; i < Graph.nodes.Count; i++)
                {
                    ChatTreeNodeData n = Graph.nodes[i];
                    if (n == null || string.IsNullOrWhiteSpace(n.Id))
                    {
                        continue;
                    }

                    var nodeView = new ChatGraphNodeView(n);
                    nodeView.SetPosition(new Rect(n.graphPosition, new Vector2(220f, 160f)));
                    AddElement(nodeView);
                    _nodeViewsById[n.Id] = nodeView;
                }

                foreach (KeyValuePair<string, ChatGraphNodeView> pair in _nodeViewsById)
                {
                    ChatGraphNodeView fromView = pair.Value;
                    ChatTreeNodeData fromData = fromView.Data;
                    if (fromData.choices == null)
                    {
                        continue;
                    }

                    for (int c = 0; c < fromData.choices.Count; c++)
                    {
                        ChatChoiceData choice = fromData.choices[c];
                        if (choice == null || string.IsNullOrWhiteSpace(choice.nextNodeId))
                        {
                            continue;
                        }

                        if (!_nodeViewsById.TryGetValue(choice.nextNodeId, out ChatGraphNodeView toView))
                        {
                            continue;
                        }

                        if (c >= fromView.OutputPorts.Count)
                        {
                            continue;
                        }

                        Port outP = fromView.OutputPorts[c];
                        Edge edge = outP.ConnectTo(toView.InputPort);
                        AddElement(edge);
                    }
                }
            }
            finally
            {
                _suppressGraphCallbacks = false;
            }
        }

        public void SaveLayoutToAsset()
        {
            if (Graph == null)
            {
                return;
            }

            Undo.RecordObject(Graph, "Chat graph layout");
            foreach (KeyValuePair<string, ChatGraphNodeView> pair in _nodeViewsById)
            {
                ChatGraphNodeView nv = pair.Value;
                Rect r = nv.GetPosition();
                nv.Data.graphPosition = new Vector2(r.x, r.y);
            }

            EditorUtility.SetDirty(Graph);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            if (_suppressGraphCallbacks)
            {
                return change;
            }

            GraphViewChange c = change;

            if (c.edgesToCreate != null)
            {
                foreach (Edge edge in c.edgesToCreate)
                {
                    WireEdge(edge);
                }
            }

            if (c.elementsToRemove != null)
            {
                foreach (GraphElement elem in c.elementsToRemove)
                {
                    if (elem is Edge edge)
                    {
                        UnwireEdge(edge);
                    }
                }
            }

            if (c.movedElements != null)
            {
                foreach (GraphElement el in c.movedElements)
                {
                    if (el is ChatGraphNodeView nv)
                    {
                        Rect r = nv.GetPosition();
                        nv.Data.graphPosition = new Vector2(r.x, r.y);
                    }
                }

                if (Graph != null)
                {
                    EditorUtility.SetDirty(Graph);
                }
            }

            return c;
        }

        private void WireEdge(Edge edge)
        {
            if (Graph == null || edge == null)
            {
                return;
            }

            Port output = edge.output;
            Port input = edge.input;
            if (output?.node is not ChatGraphNodeView fromView || input?.node is not ChatGraphNodeView toView)
            {
                return;
            }

            if (output.userData is not OutputPortKey key)
            {
                return;
            }

            ChatTreeNodeData fromData = Graph.FindNode(key.SourceNodeId);
            if (fromData?.choices == null)
            {
                return;
            }

            if (key.ChoiceIndex < 0 || key.ChoiceIndex >= fromData.choices.Count)
            {
                return;
            }

            Undo.RecordObject(Graph, "Wire chat edge");
            fromData.choices[key.ChoiceIndex].nextNodeId = toView.Data.Id;
            EditorUtility.SetDirty(Graph);
        }

        private void UnwireEdge(Edge edge)
        {
            if (Graph == null || edge?.output?.userData is not OutputPortKey key)
            {
                return;
            }

            ChatTreeNodeData fromData = Graph.FindNode(key.SourceNodeId);
            if (fromData?.choices == null)
            {
                return;
            }

            if (key.ChoiceIndex < 0 || key.ChoiceIndex >= fromData.choices.Count)
            {
                return;
            }

            Undo.RecordObject(Graph, "Unwire chat edge");
            fromData.choices[key.ChoiceIndex].nextNodeId = "";
            EditorUtility.SetDirty(Graph);
        }
    }
}
