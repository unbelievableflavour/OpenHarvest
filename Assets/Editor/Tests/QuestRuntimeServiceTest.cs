using System.Reflection;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Tests
{
    public class QuestRuntimeServiceTest
    {
        private GameObject _dbGo;
        private GameObject _serviceGo;

        [TearDown]
        public void TearDown()
        {
            if (_serviceGo != null)
            {
                Object.DestroyImmediate(_serviceGo);
                _serviceGo = null;
            }

            if (_dbGo != null)
            {
                Object.DestroyImmediate(_dbGo);
                _dbGo = null;
            }
        }

        [Test]
        public void GetVisibleNodesForNpc_FiltersByTargetNpcDefinition()
        {
            var npcA = ScriptableObject.CreateInstance<NpcInteractableDefinition>();
            var npcB = ScriptableObject.CreateInstance<NpcInteractableDefinition>();

            QuestNodeBase node = BuildSingleNodeQuest(npcA, out _, out NpcProximityInteractable interactableA, out NpcProximityInteractable interactableB);
            SetPrivateField(interactableB, "definition", npcB);

            QuestRuntimeService service = BuildServiceWithSingleQuest(node.graph as QuestGraph);
            var visibleForA = service.GetVisibleNodesForNpc(interactableA);
            var visibleForB = service.GetVisibleNodesForNpc(interactableB);

            Assert.AreEqual(1, visibleForA.Count);
            Assert.AreEqual(node, visibleForA[0]);
            Assert.AreEqual(0, visibleForB.Count);

            Object.DestroyImmediate(interactableA.gameObject);
            Object.DestroyImmediate(interactableB.gameObject);
            Object.DestroyImmediate(npcA);
            Object.DestroyImmediate(npcB);
        }

        [Test]
        public void TrySubmitGift_AdvancesPendingGiftNode()
        {
            var npcDef = ScriptableObject.CreateInstance<NpcInteractableDefinition>();
            QuestGraph graph = ScriptableObject.CreateInstance<QuestGraph>();
            EnsureGraphNodeList(graph);
            QuestGiftNode giftNode = graph.AddNode<QuestGiftNode>();
            if (giftNode == null)
            {
                giftNode = ScriptableObject.CreateInstance<QuestGiftNode>();
                giftNode.graph = graph;
                graph.nodes.Add(giftNode);
            }

            giftNode.targetNpc = npcDef;
            HarvestDataTypes.Item required = ScriptableObject.CreateInstance<HarvestDataTypes.Item>();
            required.itemId = "apple";
            giftNode.requiredItem = required;
            giftNode.requiredAmount = 2;

            QuestChatNode doneNode = graph.AddNode<QuestChatNode>();
            if (doneNode == null)
            {
                doneNode = ScriptableObject.CreateInstance<QuestChatNode>();
                doneNode.graph = graph;
                graph.nodes.Add(doneNode);
            }

            doneNode.targetNpc = npcDef;
            doneNode.completesQuest = true;

            giftNode.GetOutputPort("next").Connect(doneNode.GetInputPort("inFlow"));
            graph.entryNode = giftNode;

            var npcGo = new GameObject("Npc");
            NpcProximityInteractable interactable = npcGo.AddComponent<NpcProximityInteractable>();
            SetPrivateField(interactable, "definition", npcDef);

            QuestRuntimeService service = BuildServiceWithSingleQuest(graph);
            service.RunNodeAction(giftNode, interactionUI: null, interactable);

            Assert.IsFalse(service.TrySubmitGift(interactable, "stone", 2, out _));
            Assert.IsFalse(service.TrySubmitGift(interactable, "apple", 1, out _));
            Assert.IsTrue(service.TrySubmitGift(interactable, "apple", 2, out int requiredAmount));
            Assert.AreEqual(2, requiredAmount);

            var visibleAfterGift = service.GetVisibleNodesForNpc(interactable);
            Assert.AreEqual(1, visibleAfterGift.Count);
            Assert.AreEqual(doneNode, visibleAfterGift[0]);

            Object.DestroyImmediate(npcGo);
            Object.DestroyImmediate(required);
            Object.DestroyImmediate(npcDef);
            Object.DestroyImmediate(graph);
        }

        [Test]
        public void RequestGenericGift_HoldsOutNpcHand()
        {
            QuestGraph graph = ScriptableObject.CreateInstance<QuestGraph>();
            EnsureGraphNodeList(graph);
            QuestChatNode node = graph.AddNode<QuestChatNode>();
            if (node == null)
            {
                node = ScriptableObject.CreateInstance<QuestChatNode>();
                node.graph = graph;
                graph.nodes.Add(node);
            }

            graph.entryNode = node;
            QuestRuntimeService service = BuildServiceWithSingleQuest(graph);

            var npcDef = ScriptableObject.CreateInstance<NpcInteractableDefinition>();
            var npcGo = new GameObject("Npc");
            var controller = npcGo.AddComponent<NPCController>();
            controller.handSlot = new GameObject("HandSlot");
            controller.handSlot.SetActive(false);
            var interactable = npcGo.AddComponent<NpcProximityInteractable>();
            SetPrivateField(interactable, "definition", npcDef);

            service.RequestGenericGift(interactable);

            Assert.IsTrue(controller.handSlot.activeSelf);

            Object.DestroyImmediate(npcGo);
            Object.DestroyImmediate(npcDef);
            Object.DestroyImmediate(graph);
        }

        [Test]
        public void RunNodeAction_OnGiftNode_ShowsGiftPromptInChatUi()
        {
            var npcDef = ScriptableObject.CreateInstance<NpcInteractableDefinition>();
            QuestGraph graph = ScriptableObject.CreateInstance<QuestGraph>();
            EnsureGraphNodeList(graph);

            QuestGiftNode giftNode = graph.AddNode<QuestGiftNode>();
            if (giftNode == null)
            {
                giftNode = ScriptableObject.CreateInstance<QuestGiftNode>();
                giftNode.graph = graph;
                graph.nodes.Add(giftNode);
            }

            giftNode.targetNpc = npcDef;
            giftNode.giftPrompt = "Please hand me one apple.";
            graph.entryNode = giftNode;

            var presenterPrefab = new GameObject("GiftPromptPrefab");
            var presenter = presenterPrefab.AddComponent<NpcChatTreePresenter>();
            var bodyGo = new GameObject("Body", typeof(Text));
            bodyGo.transform.SetParent(presenterPrefab.transform, false);
            SetPrivateField(presenter, "bodyText", bodyGo.GetComponent<Text>());
            graph.chatUIPrefab = presenterPrefab;

            var uiGo = new GameObject("InteractionUi");
            var ui = uiGo.AddComponent<InteractionUIController>();
            SetPrivateField(ui, "currentInteractionRoot", uiGo.transform);

            var npcGo = new GameObject("Npc");
            var controller = npcGo.AddComponent<NPCController>();
            controller.handSlot = new GameObject("HandSlot");
            var interactable = npcGo.AddComponent<NpcProximityInteractable>();
            SetPrivateField(interactable, "definition", npcDef);

            QuestRuntimeService service = BuildServiceWithSingleQuest(graph);
            service.RunNodeAction(giftNode, ui, interactable);

            Assert.AreEqual(1, uiGo.transform.childCount);
            NpcChatTreePresenter spawnedPresenter = uiGo.transform.GetChild(0).GetComponentInChildren<NpcChatTreePresenter>(true);
            Assert.IsNotNull(spawnedPresenter);
            Text spawnedBody = uiGo.transform.GetChild(0).GetComponentInChildren<Text>(true);
            Assert.IsNotNull(spawnedBody);
            Assert.AreEqual("Please hand me one apple.", spawnedBody.text);

            Object.DestroyImmediate(uiGo);
            Object.DestroyImmediate(npcGo);
            Object.DestroyImmediate(npcDef);
            Object.DestroyImmediate(graph);
            Object.DestroyImmediate(presenterPrefab);
        }

        private QuestNodeBase BuildSingleNodeQuest(
            NpcInteractableDefinition npcDef,
            out QuestGraph graph,
            out NpcProximityInteractable interactableA,
            out NpcProximityInteractable interactableB)
        {
            graph = ScriptableObject.CreateInstance<QuestGraph>();
            EnsureGraphNodeList(graph);
            QuestChatNode node = graph.AddNode<QuestChatNode>();
            if (node == null)
            {
                node = ScriptableObject.CreateInstance<QuestChatNode>();
                node.graph = graph;
                graph.nodes.Add(node);
            }

            node.targetNpc = npcDef;
            graph.entryNode = node;

            var npcAGo = new GameObject("NpcA");
            interactableA = npcAGo.AddComponent<NpcProximityInteractable>();
            SetPrivateField(interactableA, "definition", npcDef);

            var npcBGo = new GameObject("NpcB");
            interactableB = npcBGo.AddComponent<NpcProximityInteractable>();

            return node;
        }

        private static void EnsureGraphNodeList(QuestGraph graph)
        {
            if (graph == null)
            {
                return;
            }

            if (graph.nodes == null)
            {
                graph.nodes = new List<XNode.Node>();
            }
        }

        private QuestRuntimeService BuildServiceWithSingleQuest(QuestGraph graph)
        {
            _dbGo = new GameObject("DatabaseManager");
            var dbManager = _dbGo.AddComponent<DatabaseManager>();
            var questDb = ScriptableObject.CreateInstance<QuestDatabase>();
            graph.questId = "q1";
            graph.displayName = "Quest";
            questDb.quests.Add(graph);
            dbManager.quests = questDb;
            DatabaseManager.Instance = dbManager;

            _serviceGo = new GameObject("QuestRuntimeService");
            QuestRuntimeService service = _serviceGo.AddComponent<QuestRuntimeService>();
            InvokePrivateMethod(service, "InitializeFromDatabase");
            return service;
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNotNull(field, $"Missing private field '{fieldName}' on {target.GetType().Name}");
            field.SetValue(target, value);
        }

        private static void InvokePrivateMethod(object target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNotNull(method, $"Missing private method '{methodName}' on {target.GetType().Name}");
            method.Invoke(target, null);
        }
    }
}
