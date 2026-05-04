using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Tests
{
    public class NpcChatTreePresenterTest
    {
        private GameObject _root;

        [TearDown]
        public void TearDown()
        {
            if (_root != null)
            {
                Object.DestroyImmediate(_root);
                _root = null;
            }
        }

        [Test]
        public void Begin_WithChoices_AddsActiveChoiceButtonsUnderChoiceRoot()
        {
            _root = new GameObject("ChatPresenterRoot");
            _root.SetActive(false);

            var presenter = _root.AddComponent<NpcChatTreePresenter>();

            var bodyGo = new GameObject("Body", typeof(Text));
            bodyGo.transform.SetParent(_root.transform, false);
            var bodyText = bodyGo.GetComponent<Text>();

            var choiceRootGo = new GameObject("ChoiceRoot", typeof(RectTransform));
            choiceRootGo.transform.SetParent(_root.transform, false);
            var choiceRoot = choiceRootGo.GetComponent<RectTransform>();

            var templateGo = new GameObject("Template", typeof(RectTransform), typeof(Image), typeof(Button));
            templateGo.transform.SetParent(choiceRoot.transform, false);

            SetPrivateField(presenter, "bodyText", bodyText);
            SetPrivateField(presenter, "choiceRoot", choiceRoot);
            SetPrivateField(presenter, "choiceButtonTemplate", templateGo.GetComponent<Button>());

            _root.SetActive(true);

            NpcChatGraph graph = BuildTwoChoiceGraph();
            presenter.Begin(graph, npc: null);

            int activeChoiceButtons = 0;
            for (int i = 0; i < choiceRoot.childCount; i++)
            {
                Transform child = choiceRoot.GetChild(i);
                var button = child.GetComponent<Button>();
                if (button != null && child.gameObject.activeSelf)
                {
                    activeChoiceButtons++;
                }
            }

            Assert.AreEqual(2, activeChoiceButtons);

            Object.DestroyImmediate(graph);
        }

        private static NpcChatGraph BuildTwoChoiceGraph()
        {
            var graph = ScriptableObject.CreateInstance<NpcChatGraph>();

            var entry = new ChatTreeNodeData
            {
                body = "Hello",
                choices = new List<ChatChoiceData>
                {
                    new ChatChoiceData { label = "One", nextNodeId = "b" },
                    new ChatChoiceData { label = "Two", nextNodeId = "c" },
                },
            };
            entry.SetId("a");

            var b = new ChatTreeNodeData
            {
                body = "B",
                choices = new List<ChatChoiceData>(),
            };
            b.SetId("b");

            var c = new ChatTreeNodeData
            {
                body = "C",
                choices = new List<ChatChoiceData>(),
            };
            c.SetId("c");

            graph.nodes.Add(entry);
            graph.nodes.Add(b);
            graph.nodes.Add(c);

            Assert.IsTrue(graph.TryValidate(out string err), err);

            return graph;
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNotNull(field, $"Missing private field '{fieldName}' on {target.GetType().Name}");
            field.SetValue(target, value);
        }
    }
}
