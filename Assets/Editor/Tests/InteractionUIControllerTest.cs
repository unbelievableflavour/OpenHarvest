using System.Reflection;
using System.Collections.Generic;
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

        [Test]
        public void SetDefinition_AppliesSubtitleFromDefinition()
        {
            InteractionUIController ui = CreateUi(out _, out _, out _, out _, out Text subtitleText);
            ui.enabled = false;

            var definition = ScriptableObject.CreateInstance<NpcInteractableDefinition>();
            definition.subtitle = "  Need anything?  ";

            ui.SetDefinition(definition);

            Assert.AreEqual("Need anything?", subtitleText.text);
            Assert.IsTrue(subtitleText.gameObject.activeSelf);

            Object.DestroyImmediate(definition);
        }

        [Test]
        public void SetDefinition_EmptySubtitle_HidesSubtitleField()
        {
            InteractionUIController ui = CreateUi(out _, out _, out _, out _, out Text subtitleText);
            ui.enabled = false;
            subtitleText.gameObject.SetActive(true);
            subtitleText.text = "stale";

            var definition = ScriptableObject.CreateInstance<NpcInteractableDefinition>();
            definition.subtitle = "   ";

            ui.SetDefinition(definition);

            Assert.AreEqual(string.Empty, subtitleText.text);
            Assert.IsFalse(subtitleText.gameObject.activeSelf);

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
        public System.Collections.IEnumerator SetDefinition_WithMappedActionIcon_AssignsIconOnOptionButton()
        {
            InteractionUIController ui = CreateUi(out _, out ScrollRect scrollRect, out Button optionButtonTemplate);
            ui.enabled = false;

            var iconGo = new GameObject("Icon", typeof(Image));
            iconGo.transform.SetParent(optionButtonTemplate.transform, false);
            var iconImageTemplate = iconGo.GetComponent<Image>();
            iconImageTemplate.enabled = false;

            var texture = new Texture2D(2, 2);
            Sprite mappedSprite = Sprite.Create(texture, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
            var mappings = new List<InteractionOptionIconMapping>
            {
                new InteractionOptionIconMapping
                {
                    actionType = nameof(NpcContractsInteractionOptionAction),
                    icon = mappedSprite,
                }
            };

            SetPrivateField(ui, "optionIconMappings", mappings);

            var contractsAction = ScriptableObject.CreateInstance<NpcContractsInteractionOptionAction>();
            var contractsPrefab = new GameObject("ContractsPrefab");
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
            Image iconImage = optionButton.transform.Find("Icon")?.GetComponent<Image>();
            Assert.IsNotNull(iconImage);
            Assert.IsTrue(iconImage.enabled);
            Assert.AreEqual(mappedSprite, iconImage.sprite);

            Object.DestroyImmediate(definition);
            Object.DestroyImmediate(contractsAction);
            Object.DestroyImmediate(contractsPrefab);
            Object.DestroyImmediate(mappedSprite);
            Object.DestroyImmediate(texture);

            yield return null;
        }

        [UnityTest]
        public System.Collections.IEnumerator SetDefinition_OptionRowClick_ExecutesContractsAction()
        {
            InteractionUIController ui = CreateUi(out _, out ScrollRect scrollRect, out _, out ViewSwitcher viewSwitcher, out _);
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
            InteractionUIController ui = CreateUi(out _, out _, out _, out ViewSwitcher viewSwitcher, out _);
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

        [UnityTest]
        public System.Collections.IEnumerator SetDefinition_WithInteractable_SpawnsOptionContentOnNpcCurrentInteractionRoot()
        {
            InteractionUIController ui = CreateUi(out _, out ScrollRect scrollRect, out _, out ViewSwitcher viewSwitcher, out _);
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

            var npcGo = new GameObject("Npc");
            var interactable = npcGo.AddComponent<NpcProximityInteractable>();
            Transform npcCurrentInteractionRoot = new GameObject("NpcCurrentInteractionRoot").transform;
            npcCurrentInteractionRoot.SetParent(npcGo.transform, false);
            SetPrivateField(interactable, "currentInteractionRoot", npcCurrentInteractionRoot);

            ui.SetDefinition(definition, interactable);

            Button optionButton = FindButtonByLabelText(scrollRect.content, "Contracts");
            Assert.IsNotNull(optionButton);
            optionButton.onClick.Invoke();

            Assert.AreEqual(1, npcCurrentInteractionRoot.childCount);
            Assert.AreEqual(0, currentInteractionRoot.childCount);
            Assert.IsFalse(mainViewGo.activeSelf);
            Assert.IsTrue(currentViewGo.activeSelf);

            Object.DestroyImmediate(npcGo);
            Object.DestroyImmediate(definition);
            Object.DestroyImmediate(contractsAction);
            Object.DestroyImmediate(contractsPrefab);
            Object.DestroyImmediate(mainViewGo);
            Object.DestroyImmediate(currentViewGo);

            yield return null;
        }

        [UnityTest]
        public System.Collections.IEnumerator SetDefinition_WithInteractableInFpsMode_SpawnsOnUiCurrentInteractionRoot()
        {
            InteractionUIController ui = CreateUi(out _, out ScrollRect scrollRect, out _, out ViewSwitcher viewSwitcher, out _);
            ui.enabled = false;
            var inputGo = new GameObject("HarvestInputManager");
            var inputManager = inputGo.AddComponent<HarvestInputManager>();
            var settings = ScriptableObject.CreateInstance<HarvestSettings>();
            settings.playerMode = PlayerMode.FPS;
            inputManager.harvestSettings = settings;

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

            var npcGo = new GameObject("Npc");
            var interactable = npcGo.AddComponent<NpcProximityInteractable>();
            Transform npcCurrentInteractionRoot = new GameObject("NpcCurrentInteractionRoot").transform;
            npcCurrentInteractionRoot.SetParent(npcGo.transform, false);
            SetPrivateField(interactable, "currentInteractionRoot", npcCurrentInteractionRoot);

            ui.SetDefinition(definition, interactable);

            Button optionButton = FindButtonByLabelText(scrollRect.content, "Contracts");
            Assert.IsNotNull(optionButton);
            optionButton.onClick.Invoke();

            Assert.AreEqual(0, npcCurrentInteractionRoot.childCount);
            Assert.AreEqual(1, currentInteractionRoot.childCount);
            Assert.IsFalse(mainViewGo.activeSelf);
            Assert.IsTrue(currentViewGo.activeSelf);

            Object.DestroyImmediate(npcGo);
            Object.DestroyImmediate(definition);
            Object.DestroyImmediate(contractsAction);
            Object.DestroyImmediate(contractsPrefab);
            Object.DestroyImmediate(mainViewGo);
            Object.DestroyImmediate(currentViewGo);
            Object.DestroyImmediate(inputGo);
            Object.DestroyImmediate(settings);

            yield return null;
        }

        [Test]
        public void SetDefinition_WithInteractable_SetsNpcBackToIdle()
        {
            InteractionUIController ui = CreateUi(out _, out _, out _);
            ui.enabled = false;

            var npcGo = new GameObject("Npc");
            var npcController = npcGo.AddComponent<NPCController>();
            npcController.handSlot = new GameObject("HandSlot");
            npcController.handSlot.SetActive(true);

            var interactable = npcGo.AddComponent<NpcProximityInteractable>();

            var definition = ScriptableObject.CreateInstance<NpcInteractableDefinition>();
            ui.SetDefinition(definition, interactable);

            Assert.IsFalse(npcController.handSlot.activeSelf);

            Object.DestroyImmediate(definition);
            Object.DestroyImmediate(npcGo);
        }

        [Test]
        public void SetDefinition_WithInteractable_ParentsEntireUiToNpcCurrentInteractionRoot()
        {
            InteractionUIController ui = CreateUi(out _, out _, out _);
            ui.enabled = false;

            var npcGo = new GameObject("Npc");
            var interactable = npcGo.AddComponent<NpcProximityInteractable>();
            Transform npcCurrentInteractionRoot = new GameObject("NpcCurrentInteractionRoot").transform;
            npcCurrentInteractionRoot.SetParent(npcGo.transform, false);
            SetPrivateField(interactable, "currentInteractionRoot", npcCurrentInteractionRoot);

            var definition = ScriptableObject.CreateInstance<NpcInteractableDefinition>();
            ui.SetDefinition(definition, interactable);

            Assert.AreEqual(npcCurrentInteractionRoot, ui.transform.parent);

            Object.DestroyImmediate(definition);
            Object.DestroyImmediate(npcGo);
        }

        [Test]
        public void SetDefinition_WithInteractableInFpsMode_DoesNotParentEntireUiToNpcCurrentInteractionRoot()
        {
            InteractionUIController ui = CreateUi(out _, out _, out _);
            ui.enabled = false;

            Transform originalParent = ui.transform.parent;
            var inputGo = new GameObject("HarvestInputManager");
            var inputManager = inputGo.AddComponent<HarvestInputManager>();
            var settings = ScriptableObject.CreateInstance<HarvestSettings>();
            settings.playerMode = PlayerMode.FPS;
            inputManager.harvestSettings = settings;

            var npcGo = new GameObject("Npc");
            var interactable = npcGo.AddComponent<NpcProximityInteractable>();
            Transform npcCurrentInteractionRoot = new GameObject("NpcCurrentInteractionRoot").transform;
            npcCurrentInteractionRoot.SetParent(npcGo.transform, false);
            SetPrivateField(interactable, "currentInteractionRoot", npcCurrentInteractionRoot);

            var definition = ScriptableObject.CreateInstance<NpcInteractableDefinition>();
            ui.SetDefinition(definition, interactable);

            Assert.AreEqual(originalParent, ui.transform.parent);
            Assert.AreNotEqual(npcCurrentInteractionRoot, ui.transform.parent);

            Object.DestroyImmediate(definition);
            Object.DestroyImmediate(npcGo);
            Object.DestroyImmediate(inputGo);
            Object.DestroyImmediate(settings);
        }

        [Test]
        public void SetDefinition_WhenCurrentInteractionViewIsActive_SwitchesBackToMainView()
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
            viewSwitcher.currentView = viewSwitcher.views[1];
            currentViewGo.SetActive(true);
            mainViewGo.SetActive(false);

            var definition = ScriptableObject.CreateInstance<NpcInteractableDefinition>();
            ui.SetDefinition(definition);

            Assert.AreEqual("main", viewSwitcher.currentView.id);
            Assert.IsTrue(mainViewGo.activeSelf);
            Assert.IsFalse(currentViewGo.activeSelf);

            Object.DestroyImmediate(definition);
            Object.DestroyImmediate(mainViewGo);
            Object.DestroyImmediate(currentViewGo);
        }

        [Test]
        public void OnDisable_WithInteractable_SetsNpcBackToIdle()
        {
            InteractionUIController ui = CreateUi(out _, out _, out _);

            var npcGo = new GameObject("Npc");
            var npcController = npcGo.AddComponent<NPCController>();
            npcController.handSlot = new GameObject("HandSlot");
            npcController.handSlot.SetActive(true);

            var interactable = npcGo.AddComponent<NpcProximityInteractable>();
            var definition = ScriptableObject.CreateInstance<NpcInteractableDefinition>();
            ui.SetDefinition(definition, interactable);
            npcController.handSlot.SetActive(true);

            InvokePrivateMethod(ui, "OnDisable");

            Assert.IsFalse(npcController.handSlot.activeSelf);

            Object.DestroyImmediate(definition);
            Object.DestroyImmediate(npcGo);
        }

        private InteractionUIController CreateUi(out Text npcNameText, out ScrollRect scrollRect, out Button optionButtonTemplate)
        {
            return CreateUi(out npcNameText, out scrollRect, out optionButtonTemplate, out _, out _);
        }

        private InteractionUIController CreateUi(
            out Text npcNameText,
            out ScrollRect scrollRect,
            out Button optionButtonTemplate,
            out ViewSwitcher viewSwitcher)
        {
            return CreateUi(out npcNameText, out scrollRect, out optionButtonTemplate, out viewSwitcher, out _);
        }

        private InteractionUIController CreateUi(
            out Text npcNameText,
            out ScrollRect scrollRect,
            out Button optionButtonTemplate,
            out ViewSwitcher viewSwitcher,
            out Text npcSubtitleText)
        {
            _root = new GameObject("InteractionUIControllerTestRoot");
            _root.SetActive(false);

            var ui = _root.AddComponent<InteractionUIController>();

            var npcNameGo = new GameObject("NpcNameText", typeof(Text));
            npcNameGo.transform.SetParent(_root.transform, false);
            npcNameText = npcNameGo.GetComponent<Text>();

            var npcSubtitleGo = new GameObject("NpcSubtitleText", typeof(Text));
            npcSubtitleGo.transform.SetParent(_root.transform, false);
            npcSubtitleText = npcSubtitleGo.GetComponent<Text>();

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
            SetPrivateField(ui, "npcSubtitleText", npcSubtitleText);
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

        private static void InvokePrivateMethod(object target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(method, $"Missing method {methodName}");
            method.Invoke(target, null);
        }
    }
}
