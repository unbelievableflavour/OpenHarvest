using BNG;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class HatClosetTest
    {
        GameObject instantiatedPrefab;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Configurations/HatCloset.prefab");
            instantiatedPrefab = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItChecksIfHatsCountEqualsHatSlotCount()
        {
            HatClosetController hatCloset = instantiatedPrefab.GetComponent<HatClosetController>();

            Assert.AreEqual(hatCloset.GetHatIds().Count, hatCloset.inventorySlots.childCount);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(instantiatedPrefab);
        }
    }
}
