using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NpcContractsInteractionOptionActionTest
    {
        [Test]
        public void IsValid_RequiresDisplayNameAndContractsPrefab()
        {
            var action = ScriptableObject.CreateInstance<NpcContractsInteractionOptionAction>();
            var contractsPrefab = new GameObject("ContractsPrefab");

            var optionMissingPrefab = new NpcInteractionOption { displayName = "Contracts", action = action };
            Assert.IsFalse(action.IsValid(optionMissingPrefab));

            SetPrivateField(action, "contractsPrefab", contractsPrefab);

            var optionMissingName = new NpcInteractionOption { displayName = "", action = action };
            Assert.IsFalse(action.IsValid(optionMissingName));

            var optionValid = new NpcInteractionOption { displayName = "Contracts", action = action };
            Assert.IsTrue(action.IsValid(optionValid));

            Object.DestroyImmediate(contractsPrefab);
            Object.DestroyImmediate(action);
        }

        [UnityTest]
        public System.Collections.IEnumerator Execute_InstantiatesContractsPrefabUnderCurrentInteractionRoot()
        {
            var root = new GameObject("InteractionUIRoot");
            var current = new GameObject("CurrentInteractionRoot").transform;
            current.SetParent(root.transform, false);

            var ui = root.AddComponent<InteractionUIController>();
            SetPrivateField(ui, "currentInteractionRoot", current);

            var action = ScriptableObject.CreateInstance<NpcContractsInteractionOptionAction>();
            var contractsPrefab = new GameObject("ContractsPrefab");
            SetPrivateField(action, "contractsPrefab", contractsPrefab);

            var option = new NpcInteractionOption { displayName = "Contracts", action = action };

            action.Execute(ui, null, option);

            Assert.AreEqual(1, current.childCount);

            yield return null;

            Object.DestroyImmediate(contractsPrefab);
            Object.DestroyImmediate(action);
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
