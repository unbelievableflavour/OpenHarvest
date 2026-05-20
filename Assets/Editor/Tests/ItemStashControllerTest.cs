using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class ItemStashControllerTest
    {
        const string PlaceableInstanceId = "placed-chest-instance-1";

        [SetUp]
        public void SetUp()
        {
            GameState.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            ResetStoredItems();
            GameState.Reset();
        }

        [Test]
        public void ItUsesItemStashNameWhenUsePlaceableIdIsDisabled()
        {
            ItemStashController controller = CreateController(usePlaceableId: false, withPlaceableInstanceId: true);
            InvokeResolveStashKey(controller);

            Assert.AreEqual("storageCrate", GetActiveStashKey(controller));
        }

        [Test]
        public void ItUsesPlaceableInstanceIdWhenUsePlaceableIdIsEnabled()
        {
            ItemStashController controller = CreateController(usePlaceableId: true, withPlaceableInstanceId: true);
            InvokeResolveStashKey(controller);

            Assert.AreEqual(PlaceableInstanceId, GetActiveStashKey(controller));
        }

        [Test]
        public void ItFallsBackToItemStashNameWhenPlaceableInstanceIdIsMissing()
        {
            ItemStashController controller = CreateController(usePlaceableId: true, withPlaceableInstanceId: false);
            InvokeResolveStashKey(controller);

            Assert.AreEqual("storageCrate", GetActiveStashKey(controller));
        }

        [Test]
        public void ItReadsAndWritesInventoryUsingPlaceableInstanceId()
        {
            ItemStashController controller = CreateController(usePlaceableId: true, withPlaceableInstanceId: true);
            InvokeResolveStashKey(controller);

            var expectedItems = new List<SaveableItem>
            {
                new SaveableItem { id = "Tomato", currentStackSize = 2 }
            };
            GameState.Instance.itemStashes[PlaceableInstanceId] = expectedItems;

            List<SaveableItem> loadedItems = InvokeGetFromGameState(controller);

            Assert.AreEqual(1, loadedItems.Count);
            Assert.AreEqual("Tomato", loadedItems[0].id);
            Assert.AreEqual(2, loadedItems[0].currentStackSize);

            InvokeSetInGameState(controller, new List<SaveableItem>
            {
                new SaveableItem { id = "Carrot", currentStackSize = 1 }
            });

            Assert.IsTrue(GameState.Instance.itemStashes.ContainsKey(PlaceableInstanceId));
            Assert.AreEqual("Carrot", GameState.Instance.itemStashes[PlaceableInstanceId][0].id);
            Assert.IsFalse(GameState.Instance.itemStashes.ContainsKey("storageCrate"));
        }

        [Test]
        public void ItReturnsEmptyInventoryWhenPlaceableStashDoesNotExistYet()
        {
            ItemStashController controller = CreateController(usePlaceableId: true, withPlaceableInstanceId: true);
            InvokeResolveStashKey(controller);

            List<SaveableItem> loadedItems = InvokeGetFromGameState(controller);

            Assert.IsNotNull(loadedItems);
            Assert.AreEqual(0, loadedItems.Count);
        }

        private static ItemStashController CreateController(bool usePlaceableId, bool withPlaceableInstanceId)
        {
            var root = new GameObject("PlacedStorageChest");
            if (withPlaceableInstanceId)
            {
                var placedId = root.AddComponent<PlacedObjectInstanceId>();
                placedId.instanceId = PlaceableInstanceId;
            }

            var controllerObject = new GameObject("Inventory");
            controllerObject.transform.SetParent(root.transform);
            var controller = controllerObject.AddComponent<ItemStashController>();
            controller.itemStashName = "storageCrate";
            controller.usePlaceableId = usePlaceableId;
            controller.inventorySlots = new GameObject("Slots").transform;
            controller.inventorySlots.SetParent(controllerObject.transform);
            return controller;
        }

        private static void InvokeResolveStashKey(ItemStashController controller)
        {
            InvokePrivateMethod(controller, "ResolveStashKey");
        }

        private static string GetActiveStashKey(ItemStashController controller)
        {
            return (string)InvokePrivateMethod(controller, "GetActiveStashKey");
        }

        private static List<SaveableItem> InvokeGetFromGameState(ItemStashController controller)
        {
            return (List<SaveableItem>)InvokePrivateMethod(controller, "GetFromGameState");
        }

        private static void InvokeSetInGameState(ItemStashController controller, List<SaveableItem> items)
        {
            SetPrivateField(controller, "storedItems", items);
            InvokePrivateMethod(controller, "SetInGameState");
        }

        private static object InvokePrivateMethod(ItemStashController controller, string methodName)
        {
            MethodInfo method = typeof(ItemStashController).GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(method, "Expected private method: " + methodName);
            return method.Invoke(controller, null);
        }

        private static void SetPrivateField(ItemStashController controller, string fieldName, object value)
        {
            FieldInfo field = typeof(ItemStashController).GetField(
                fieldName,
                BindingFlags.Static | BindingFlags.NonPublic);
            Assert.IsNotNull(field, "Expected static field: " + fieldName);
            field.SetValue(null, value);
        }

        private static void ResetStoredItems()
        {
            SetPrivateField(null, "storedItems", new List<SaveableItem>());
        }
    }
}
