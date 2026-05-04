using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OpenHarvest.EditorTools.ChatGraph
{
    public sealed class NpcChatGraphEditorWindow : EditorWindow
    {
        private const float ScrollCanvasSize = 12000f;

        private NpcChatGraph _graph;
        private ChatGraphView _graphView;
        private ScrollView _graphScroll;
        private ObjectField _graphField;

        [MenuItem("Window/OpenHarvest/NPC Chat Graph")]
        public static void ShowWindow()
        {
            NpcChatGraphEditorWindow win = GetWindow<NpcChatGraphEditorWindow>();
            win.titleContent = new GUIContent("NPC Chat Graph");
            win.Show();
        }

        public static void Open(NpcChatGraph graph)
        {
            NpcChatGraphEditorWindow win = GetWindow<NpcChatGraphEditorWindow>();
            win.titleContent = new GUIContent("NPC Chat Graph");
            win._graph = graph;
            if (win._graphField != null)
            {
                win._graphField.SetValueWithoutNotify(graph);
            }

            if (win._graphView != null)
            {
                win.RebuildGraph();
            }

            win.Show();
        }

        private void OnEnable()
        {
            BuildUi();
        }

        private void OnDisable()
        {
            if (_graphView != null)
            {
                _graphView.SaveLayoutToAsset();
            }
        }

        private void BuildUi()
        {
            rootVisualElement.Clear();

            var root = new VisualElement();
            root.style.flexGrow = 1;
            root.style.flexDirection = FlexDirection.Column;

            var toolbar = new Toolbar();

            _graphField = new ObjectField("Chat graph")
            {
                objectType = typeof(NpcChatGraph),
                value = _graph,
            };
            _graphField.RegisterValueChangedCallback(evt =>
            {
                _graph = evt.newValue as NpcChatGraph;
                RebuildGraph();
            });
            toolbar.Add(_graphField);

            var addBtn = new ToolbarButton(AddNode)
            {
                text = "Add node",
            };
            toolbar.Add(addBtn);

            var refreshBtn = new ToolbarButton(RebuildGraph)
            {
                text = "Reload",
            };
            toolbar.Add(refreshBtn);

            root.Add(toolbar);

            var graphScroll = new ScrollView(ScrollViewMode.VerticalAndHorizontal)
            {
                horizontalScrollerVisibility = ScrollerVisibility.AlwaysVisible,
                verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible,
            };
            graphScroll.style.flexGrow = 1;
            _graphScroll = graphScroll;

            var scrollCanvas = new VisualElement();
            scrollCanvas.style.width = ScrollCanvasSize;
            scrollCanvas.style.height = ScrollCanvasSize;
            scrollCanvas.style.position = Position.Relative;

            _graphView = new ChatGraphView();
            _graphView.style.position = Position.Absolute;
            _graphView.style.left = 0f;
            _graphView.style.top = 0f;
            _graphView.style.width = ScrollCanvasSize;
            _graphView.style.height = ScrollCanvasSize;
            scrollCanvas.Add(_graphView);
            graphScroll.Add(scrollCanvas);
            root.Add(graphScroll);

            rootVisualElement.Add(root);
            RebuildGraph();
            ScrollToGraphOrigin();
        }

        private void AddNode()
        {
            if (_graph == null)
            {
                return;
            }

            Undo.RecordObject(_graph, "Add chat node");
            var node = new ChatTreeNodeData
            {
                body = "New line",
                choices = new System.Collections.Generic.List<ChatChoiceData>
                {
                    new ChatChoiceData { label = "Next", nextNodeId = "" },
                },
            };
            node.SetId("node_" + Guid.NewGuid().ToString("N").Substring(0, 8));
            _graph.nodes.Add(node);
            EditorUtility.SetDirty(_graph);
            RebuildGraph();
        }

        private void RebuildGraph()
        {
            if (_graphView == null)
            {
                return;
            }

            _graphView.Graph = _graph;
            _graphView.RebuildFromGraph();
        }

        private void ScrollToGraphOrigin()
        {
            if (_graphScroll == null)
            {
                return;
            }

            _graphScroll.schedule.Execute(() =>
            {
                _graphScroll.scrollOffset = Vector2.zero;
            });
        }
    }
}
