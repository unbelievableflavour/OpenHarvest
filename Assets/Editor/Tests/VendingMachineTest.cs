using BNG;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Tests
{
    public class VendingMachineTest
    {
        GameObject instantiatedPrefab;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/VendingMachine/VendingMachine.prefab");
            instantiatedPrefab = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItChecksIfAllRequiredFieldsForSpawnControllerAreNotEmpty()
        {
            var spawnController = instantiatedPrefab.GetComponentInChildren<SpawnController>();

            Assert.AreNotEqual(null, spawnController.spawnLocation);
            Assert.AreNotEqual(null, spawnController.spawnEffect);
            Assert.AreNotEqual(null, spawnController.spawnEffectLocation);
            Assert.AreNotEqual(null, spawnController.selectedItemLabel);
            Assert.AreNotEqual(null, spawnController.leverSound);
        }

        [Test]
        public void ItChecksIfAllRequiredFieldsForSpawnerItemsListerAreNotEmpty()
        {
            var spawnersItemLister = instantiatedPrefab.GetComponentInChildren<SpawnerItemsLister>();

            Assert.AreNotEqual(null, spawnersItemLister.itemRowPrefab);
            Assert.AreNotEqual(null, spawnersItemLister.functionalScrollViewContent);
            Assert.AreNotEqual(null, spawnersItemLister.spawnController);
        }

        [Test]
        public void ItChecksTheUISettingsAreCorrectlySet()
        {
            var scrollRect = instantiatedPrefab.GetComponentInChildren<ScrollRect>();

            Assert.AreEqual(10, scrollRect.scrollSensitivity);
            Assert.AreEqual(0.0941176489f, scrollRect.verticalScrollbar.GetComponent<Image>().color.a);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(instantiatedPrefab);
        }
    }
}
