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
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Configurations/HatCloset.prefab");
            instantiatedPrefab = GameObject.Instantiate(prefab);

            StartDatabaseManager();
        }

        [Test]
        public void ItChecksIfHatsCountEqualsHatSlotCount()
        {
            HatClosetController hatCloset = instantiatedPrefab.GetComponent<HatClosetController>();
            int hatCount = DatabaseManager.Instance.items.FindAllByTag("hatCloset").Count;
            
            Assert.AreEqual(hatCount, 0);
            Assert.AreEqual(hatCount, hatCloset.inventorySlots.childCount);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(instantiatedPrefab);
        }
                    
        void StartDatabaseManager()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ScriptableObjects/Databases/DatabaseManager/DatabaseManager.prefab");
            databaseManager = GameObject.Instantiate(prefab);
            DatabaseManager.Instance = databaseManager.GetComponent<DatabaseManager>();
        }
    }
}
