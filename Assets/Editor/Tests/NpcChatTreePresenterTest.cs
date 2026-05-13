using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using XNode;

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
            presenter.Begin(graph.entryNode, npc: null);

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

        [Test]
        public void BeginSingleLine_WithAutoTypeBody_SetsAutoTypeMessage()
        {
            _root = new GameObject("ChatPresenterRoot");
            _root.SetActive(false);

            var presenter = _root.AddComponent<NpcChatTreePresenter>();

            var bodyGo = new GameObject("Body");
            bodyGo.transform.SetParent(_root.transform, false);
            var bodyText = bodyGo.AddComponent<Text>();
            bodyText.text = "PLACEHOLDER";
            var bodyAutoType = bodyGo.AddComponent<AutoType>();

            var choiceRootGo = new GameObject("ChoiceRoot", typeof(RectTransform));
            choiceRootGo.transform.SetParent(_root.transform, false);
            var choiceRoot = choiceRootGo.GetComponent<RectTransform>();

            var templateGo = new GameObject("Template", typeof(RectTransform), typeof(Image), typeof(Button));
            templateGo.transform.SetParent(choiceRoot.transform, false);

            SetPrivateField(presenter, "bodyText", bodyText);
            SetPrivateField(presenter, "choiceRoot", choiceRoot);
            SetPrivateField(presenter, "choiceButtonTemplate", templateGo.GetComponent<Button>());

            _root.SetActive(true);

            presenter.BeginSingleLine("Howdy", npc: null, showContinue: false);

            Assert.AreEqual("Howdy", bodyAutoType.GetMessage());
        }

        private static NpcChatGraph BuildTwoChoiceGraph()
        {
            var graph = ScriptableObject.CreateInstance<NpcChatGraph>();
            if (graph.nodes == null)
            {
                graph.nodes = new List<Node>();
            }

            NpcChatNode entry = graph.AddNode<NpcChatNode>();
            if (entry == null)
            {
                entry = ScriptableObject.CreateInstance<NpcChatNode>();
                entry.graph = graph;
                graph.nodes.Add(entry);
            }

            entry.body = "Hello";
            entry.choices = new List<string> { "One", "Two" };

            NpcChatNode b = graph.AddNode<NpcChatNode>();
            if (b == null)
            {
                b = ScriptableObject.CreateInstance<NpcChatNode>();
                b.graph = graph;
                graph.nodes.Add(b);
            }

            b.body = "B";

            NpcChatNode c = graph.AddNode<NpcChatNode>();
            if (c == null)
            {
                c = ScriptableObject.CreateInstance<NpcChatNode>();
                c.graph = graph;
                graph.nodes.Add(c);
            }

            c.body = "C";

            Assert.IsNotNull(entry);
            graph.entryNode = entry;

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
