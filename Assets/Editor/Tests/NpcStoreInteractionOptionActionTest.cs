using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NpcStoreInteractionOptionActionTest
    {
        [Test]
        public void IsValid_RequiresStoreNameAndAtLeastOneStoreId()
        {
            var action = ScriptableObject.CreateInstance<NpcStoreInteractionOptionAction>();
            var storePrefab = new GameObject("StorePrefab");

            SetPrivateField(action, "storePrefab", storePrefab);
            SetPrivateField(action, "storeName", "General Store");
            SetPrivateField(action, "storeIdFunctional", "");
            SetPrivateField(action, "storeIdDecorational", "");

            var option = new NpcInteractionOption { displayName = "Shop", action = action };

            Assert.IsFalse(action.IsValid(option));

            SetPrivateField(action, "storeIdFunctional", "functional_store");
            Assert.IsTrue(action.IsValid(option));

            SetPrivateField(action, "storeName", "");
            Assert.IsFalse(action.IsValid(option));

            Object.DestroyImmediate(storePrefab);
            Object.DestroyImmediate(action);
        }

        [UnityTest]
        public System.Collections.IEnumerator ShowInstancedOptionContent_InvokesAfterInstantiateCallback()
        {
            var root = new GameObject("InteractionUIRoot");
            var current = new GameObject("CurrentInteractionRoot").transform;
            current.SetParent(root.transform, false);

            var ui = root.AddComponent<InteractionUIController>();
            SetPrivateField(ui, "currentInteractionRoot", current);

            var prefab = new GameObject("Prefab");

            GameObject captured = null;
            ui.ShowInstancedOptionContent(prefab, go => captured = go);

            Assert.IsNotNull(captured);
            Assert.AreEqual(current, captured.transform.parent);

            yield return null;

            Object.DestroyImmediate(prefab);
            Object.DestroyImmediate(root);
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
