using BNG;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class HatClosetTest
    {
        GameObject instantiatedPrefab;
        GameObject databaseManager;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PlaceableObjects/HomeCloset/HomeCloset.prefab");
            Assert.IsNotNull(prefab, "Hat closet prefab not found at expected path.");
            instantiatedPrefab = GameObject.Instantiate(prefab);
            Assert.IsNotNull(
                instantiatedPrefab.GetComponentInChildren<HatClosetController>(true),
                "HatClosetController is missing in HomeCloset prefab hierarchy.");

            StartDatabaseManager();
        }

        [Test]
        public void ItChecksIfHatsCountEqualsHatSlotCount()
        {
            HatClosetController hatCloset = instantiatedPrefab.GetComponentInChildren<HatClosetController>(true);
            Assert.IsNotNull(hatCloset, "HatClosetController not found in instantiated prefab children.");
            int hatCount = DatabaseManager.Instance.items.FindAllByTag("hatCloset").Count;
            
            Assert.AreEqual(hatCount, 21);
            Assert.AreEqual(hatCount, hatCloset.inventorySlots.childCount);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(instantiatedPrefab);
            Object.DestroyImmediate(databaseManager);
        }
                    
        void StartDatabaseManager()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/_Core/DatabaseManager/DatabaseManager.prefab");
            databaseManager = GameObject.Instantiate(prefab);
            DatabaseManager.Instance = databaseManager.GetComponent<DatabaseManager>();
        }
    }
}
