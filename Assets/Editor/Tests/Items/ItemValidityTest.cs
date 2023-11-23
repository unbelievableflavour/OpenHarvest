using NUnit.Framework;
using UnityEngine;
using System.Linq;
using HarvestDataTypes;
using UnityEditor;
using BNG;

namespace Items
{
    public class ItemValidityTest
    {
        GameObject databaseManager;
        ItemDatabase itemDatabase;

        [SetUp]
        public void SetUp()
        {
            StartDatabaseManager();
            GameState.Reset();
        }

        [Test]
        public void ItHasAFilledDatabase()
        {
            Assert.AreEqual(true, itemCountIsHigherThan200());
        }

        [Test]
        public void ItChecksAllItemsForValidity()
        {
            Assert.AreEqual(true, itemCountIsHigherThan200());

            foreach (var item in itemDatabase.items)
            {
                Assert.AreEqual(false, string.IsNullOrEmpty(item.name), "item: \"" + item.itemId + "\" is invalid. Title value is empty");
            }
        }

        [Test]
        public void ItCanInstantiateEveryItem()
        {
            foreach (HarvestDataTypes.Item item in itemDatabase.items)
            {
                if (item.prefab == null)
                {
                    continue;
                }

                GameObject spawnedItem = Definitions.InstantiateItemNew(item.prefab);

                Assert.AreEqual(false, spawnedItem.GetComponents<GrabbableHaptics>().Length == 2, spawnedItem.gameObject.name + " has 2 grabbableHaptics components");
                
                Object.DestroyImmediate(spawnedItem);
            }
        }

        [Test]
        public void ItHasScale1AndRot0AndPos0ForPrefabItems()
        {
            foreach (var item in itemDatabase.items)
            {
                if (item.prefab == null)
                {
                    continue;
                }

                GameObject spawnedItem = Definitions.InstantiateItemNew(item.prefab);
                Assert.AreEqual(0 , spawnedItem.transform.position.x, "Invalid position X for item: " + spawnedItem.transform.name);
                Assert.AreEqual(0, spawnedItem.transform.position.y, "Invalid position Y for item: " + spawnedItem.transform.name);
                Assert.AreEqual(0, spawnedItem.transform.position.z, "Invalid position Z for item: " + spawnedItem.transform.name);
                Assert.AreEqual(0, spawnedItem.transform.rotation.x, "Invalid rotation X for item: " + spawnedItem.transform.name);
                Assert.AreEqual(0, spawnedItem.transform.rotation.y, "Invalid rotation Y for item: " + spawnedItem.transform.name);
                Assert.AreEqual(0, spawnedItem.transform.rotation.z, "Invalid rotation Z for item: " + spawnedItem.transform.name);
                Assert.AreEqual(1.0f, spawnedItem.transform.localScale.x, "Invalid scale X for item: " + spawnedItem.transform.name);
                Assert.AreEqual(1.0f, spawnedItem.transform.localScale.y, "Invalid scale Y for item: " + spawnedItem.transform.name);
                Assert.AreEqual(1.0f, spawnedItem.transform.localScale.z, "Invalid scale Z for item: " + spawnedItem.transform.name);
                Object.DestroyImmediate(spawnedItem);
            }
        }

        [Test]
        public void ItHasCorrectRigidBodyOptions()
        {
            foreach (var item in itemDatabase.items)
            {
                if (item.prefab == null)
                {
                    continue;
                }

                //Weird wallet, skip for now
                if(item.name == "Wallet"){
                    continue;
                }

                GameObject spawnedItem = Definitions.InstantiateItemNew(item.prefab);
                Rigidbody rigidBody = spawnedItem.GetComponent<Rigidbody>();
                Assert.AreEqual(CollisionDetectionMode.ContinuousDynamic, rigidBody.collisionDetectionMode, "Invalid rigidBody collision detection for item: " + spawnedItem.transform.name);
                Object.DestroyImmediate(spawnedItem);
            }
        }

        [Test]
        public void ItHasCorrectGrabbableOptions()
        {
            string[] listOfWeirdItems = { 
                "Wallet", // not tested. no time
                "Basket", // not tested. no time
                "FishingRod", // 2-handed
                "Hammer", // 2-handed
                "LargeAxe", // 2-handed
                "Pickaxe", // 2-handed
                "PickaxeIron", // 2-handed
            };

            foreach (var item in itemDatabase.items)
            {
                if (item.prefab == null)
                {
                    continue;
                }

                //Exceptional items, skip for now
                if(listOfWeirdItems.Contains(item.itemId)) {
                    continue;
                }

                GameObject spawnedItem = Definitions.InstantiateItemNew(item.prefab);
                Grabbable grabbable = spawnedItem.GetComponent<Grabbable>();
                Assert.AreEqual(GrabPhysics.PhysicsJoint, grabbable.GrabPhysics, "Invalid grab physics for item: " + spawnedItem.transform.name);
                Assert.AreEqual(20, grabbable.GrabSpeed, "Invalid grab speed for item: " + spawnedItem.transform.name);
                Assert.AreEqual(2.5, grabbable.RemoteGrabDistance, "Invalid remote grab distance for item: " + spawnedItem.transform.name);
                Object.DestroyImmediate(spawnedItem);
            }
        }

        [TearDown]
        public void Cleanup()
        {
            GameState.Reset();
            Object.DestroyImmediate(databaseManager);
        }

        public bool itemCountIsHigherThan200()
        {
            return itemDatabase.items.Count > 200;
        }

        void StartDatabaseManager()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/_Core/DatabaseManager/DatabaseManager.prefab");
            databaseManager = GameObject.Instantiate(prefab);
            itemDatabase = databaseManager.GetComponent<DatabaseManager>().items;
        }
    }
}
