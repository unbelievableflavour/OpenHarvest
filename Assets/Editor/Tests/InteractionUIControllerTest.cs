using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

namespace Tests
{
    public class InteractionUIControllerTest
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
        public void SetDefinition_WhenDefinitionNull_SetsDashNpcName()
        {
            InteractionUIController ui = CreateUi(out Text npcNameText, out _, out _);
            ui.enabled = false;

            npcNameText.text = "unchanged";
            ui.SetDefinition(null);

            Assert.AreEqual("—", npcNameText.text);
        }

        [Test]
        public void SetDefinition_SetsNpcNameFromDefinition()
        {
            InteractionUIController ui = CreateUi(out Text npcNameText, out _, out _);
            ui.enabled = false;

            var definition = ScriptableObject.CreateInstance<NpcInteractableDefinition>();
            definition.npcName = "Pig";

            ui.SetDefinition(definition);

            Assert.AreEqual("Pig", npcNameText.text);

            Object.DestroyImmediate(definition);
        }

        [UnityTest]
        public System.Collections.IEnumerator SetDefinition_CreatesOneRowPerValidOptionPlusGoodbye()
        {
            InteractionUIController ui = CreateUi(out _, out ScrollRect scrollRect, out Button optionButtonTemplate);
            ui.enabled = false;

            var contractsAction = ScriptableObject.CreateInstance<NpcContractsInteractionOptionAction>();
            var contractsPrefab = new GameObject("ContractsPrefab");
            SetPrivateField(contractsAction, "contractsPrefab", contractsPrefab);

            var definition = ScriptableObject.CreateInstance<NpcInteractableDefinition>();
            definition.options.Add(new NpcInteractionOption
            {
                displayName = "Hello",
                action = contractsAction,
            });
            definition.options.Add(new NpcInteractionOption
            {
                displayName = "Invalid",
                action = null,
            });

            ui.SetDefinition(definition);

            Assert.AreEqual(2, scrollRect.content.childCount);

            Object.DestroyImmediate(definition);
            Object.DestroyImmediate(contractsAction);
            Object.DestroyImmediate(contractsPrefab);

            yield return null;
        }

        [UnityTest]
        public System.Collections.IEnumerator SetDefinition_OptionRowClick_ExecutesContractsAction()
        {
            InteractionUIController ui = CreateUi(out _, out ScrollRect scrollRect, out _, out ViewSwitcher viewSwitcher);
            ui.enabled = false;

            GameObject mainViewGo = new GameObject("main_view");
            GameObject currentViewGo = new GameObject("current_view");
            mainViewGo.SetActive(true);
            currentViewGo.SetActive(true);
            viewSwitcher.views.Clear();
            viewSwitcher.views.Add(new View { id = "main", view = mainViewGo });
            viewSwitcher.views.Add(new View { id = "currentInteraction", view = currentViewGo });
            viewSwitcher.currentView = viewSwitcher.views[0];

            Transform currentInteractionRoot = viewSwitcher.transform.Find("CurrentInteractionRoot");

            var contractsPrefab = new GameObject("ContractsPrefab");
            var contractsAction = ScriptableObject.CreateInstance<NpcContractsInteractionOptionAction>();
            SetPrivateField(contractsAction, "contractsPrefab", contractsPrefab);

            var definition = ScriptableObject.CreateInstance<NpcInteractableDefinition>();
            definition.options.Add(new NpcInteractionOption
            {
                displayName = "Contracts",
                action = contractsAction,
            });

            ui.SetDefinition(definition);

            Button optionButton = FindButtonByLabelText(scrollRect.content, "Contracts");
            Assert.IsNotNull(optionButton);
            optionButton.onClick.Invoke();

            Assert.AreEqual(1, currentInteractionRoot.childCount);
            Assert.IsFalse(mainViewGo.activeSelf);
            Assert.IsTrue(currentViewGo.activeSelf);

            Object.DestroyImmediate(definition);
            Object.DestroyImmediate(contractsAction);
            Object.DestroyImmediate(contractsPrefab);
            Object.DestroyImmediate(mainViewGo);
            Object.DestroyImmediate(currentViewGo);

            yield return null;
        }

        [UnityTest]
        public System.Collections.IEnumerator ShowInstancedOptionContent_SwitchesToCurrentInteractionView()
        {
            InteractionUIController ui = CreateUi(out _, out _, out _, out ViewSwitcher viewSwitcher);
            ui.enabled = false;

            GameObject mainViewGo = new GameObject("main_view");
            GameObject currentViewGo = new GameObject("current_view");
            mainViewGo.SetActive(true);
            currentViewGo.SetActive(true);

            viewSwitcher.views.Clear();
            viewSwitcher.views.Add(new View { id = "main", view = mainViewGo });
            viewSwitcher.views.Add(new View { id = "currentInteraction", view = currentViewGo });
            viewSwitcher.currentView = viewSwitcher.views[0];

            var prefab = new GameObject("ContractsPrefab");

            ui.ShowInstancedOptionContent(prefab);

            Assert.IsFalse(mainViewGo.activeSelf);
            Assert.IsTrue(currentViewGo.activeSelf);

            Object.DestroyImmediate(prefab);
            Object.DestroyImmediate(mainViewGo);
            Object.DestroyImmediate(currentViewGo);

            yield return null;
        }

        private InteractionUIController CreateUi(out Text npcNameText, out ScrollRect scrollRect, out Button optionButtonTemplate)
        {
            return CreateUi(out npcNameText, out scrollRect, out optionButtonTemplate, out _);
        }

        private InteractionUIController CreateUi(
            out Text npcNameText,
            out ScrollRect scrollRect,
            out Button optionButtonTemplate,
            out ViewSwitcher viewSwitcher)
        {
            _root = new GameObject("InteractionUIControllerTestRoot");
            _root.SetActive(false);

            var ui = _root.AddComponent<InteractionUIController>();

            var npcNameGo = new GameObject("NpcNameText", typeof(Text));
            npcNameGo.transform.SetParent(_root.transform, false);
            npcNameText = npcNameGo.GetComponent<Text>();

            var scrollGo = new GameObject("ScrollRect", typeof(ScrollRect));
            scrollGo.transform.SetParent(_root.transform, false);
            scrollRect = scrollGo.GetComponent<ScrollRect>();

            var viewport = new GameObject("Viewport", typeof(RectTransform));
            viewport.transform.SetParent(scrollGo.transform, false);
            scrollRect.viewport = viewport.GetComponent<RectTransform>();

            var content = new GameObject("Content", typeof(RectTransform));
            content.transform.SetParent(viewport.transform, false);
            scrollRect.content = content.GetComponent<RectTransform>();

            var templateGo = new GameObject("OptionButtonTemplate", typeof(RectTransform), typeof(Image), typeof(Button));
            templateGo.transform.SetParent(_root.transform, false);
            optionButtonTemplate = templateGo.GetComponent<Button>();

            var labelGo = new GameObject("Label", typeof(Text));
            labelGo.transform.SetParent(templateGo.transform, false);
            labelGo.GetComponent<Text>().text = "template";

            var viewSwitcherGo = new GameObject("ViewSwitcher", typeof(ViewSwitcher));
            viewSwitcherGo.transform.SetParent(_root.transform, false);
            viewSwitcher = viewSwitcherGo.GetComponent<ViewSwitcher>();

            var currentInteractionRoot = new GameObject("CurrentInteractionRoot").transform;
            currentInteractionRoot.SetParent(viewSwitcherGo.transform, false);

            SetPrivateField(ui, "npcNameText", npcNameText);
            SetPrivateField(ui, "scrollRect", scrollRect);
            SetPrivateField(ui, "optionButtonTemplate", optionButtonTemplate);
            SetPrivateField(ui, "viewSwitcher", viewSwitcher);
            SetPrivateField(ui, "currentInteractionRoot", currentInteractionRoot);

            _root.SetActive(true);

            return ui;
        }

        private static Button FindButtonByLabelText(Transform contentRoot, string label)
        {
            for (int i = 0; i < contentRoot.childCount; i++)
            {
                Transform row = contentRoot.GetChild(i);
                var labelText = row.GetComponentInChildren<Text>(true);
                if (labelText != null && labelText.text == label)
                {
                    return row.GetComponentInParent<Button>();
                }
            }

            return null;
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
