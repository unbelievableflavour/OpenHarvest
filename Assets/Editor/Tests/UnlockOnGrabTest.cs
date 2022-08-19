using BNG;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HarvestDataTypes;

namespace Tests
{
    public class UnlockOnGrabTest
    {
        string resourcePath = "Assets/Resources/Items/HatChicken/HatChicken.prefab";
        HarvestDataTypes.Item item;
        string itemId = "HatChicken";
        Grabber fakeGrabber = null;
        GameObject databaseManager;

        [SetUp]
        public void SetUp()
        {
            item = new HarvestDataTypes.Item();
            item.itemId = itemId;
            Cleanup();

            StartDatabaseManager();
            var itemLoader = new ItemLoader();
            itemLoader.Start();

            //Definitions.ItemsWithInformation[itemId].maximumTimesOwned = 2;
        }

        [Test]
        public void ItUnlocksTheItemOnGrabbing()
        {
            Assert.AreEqual(0, GameState.Instance.unlockables[itemId]);

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(resourcePath);
            prefab = GameObject.Instantiate(prefab);
            prefab.GetComponent<ItemInformation>().item = item;
            prefab.GetComponent<UnlockOnGrab>().unlockSound = null;
            prefab.GetComponent<UnlockOnGrab>().OnGrab(fakeGrabber);

            Assert.AreEqual(1, GameState.Instance.unlockables[itemId]);
        }

        [Test]
        public void ItWontUnlockTwiceWhenGrabbingItAgain()
        {
            Assert.AreEqual(0, GameState.Instance.unlockables[itemId]);

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(resourcePath);
            prefab = GameObject.Instantiate(prefab);
            prefab.GetComponent<ItemInformation>().item = item;
            prefab.GetComponent<UnlockOnGrab>().unlockSound = null;
            prefab.GetComponent<UnlockOnGrab>().OnGrab(fakeGrabber);

            Assert.AreEqual(1, GameState.Instance.unlockables[itemId]);

            prefab.GetComponent<UnlockOnGrab>().OnGrab(fakeGrabber);

            Assert.AreEqual(1, GameState.Instance.unlockables[itemId]);
        }

        [Test]
        public void ItWontUnlockMoreThanItemsMaximumTimesOwned()
        {
            Assert.AreEqual(0, GameState.Instance.unlockables[itemId]);

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(resourcePath);
            prefab = GameObject.Instantiate(prefab);
            prefab.GetComponent<ItemInformation>().item = item;
            prefab.GetComponent<UnlockOnGrab>().unlockSound = null;
            prefab.GetComponent<UnlockOnGrab>().OnGrab(fakeGrabber);

            Assert.AreEqual(true, GameState.Instance.unlockables.ContainsKey(itemId));
            Assert.AreEqual(1, GameState.Instance.unlockables[itemId]);

            prefab = GameObject.Instantiate(prefab);
            prefab.GetComponent<ItemInformation>().item = item;
            prefab.GetComponent<UnlockOnGrab>().unlockSound = null;
            prefab.GetComponent<UnlockOnGrab>().OnGrab(fakeGrabber);

            Assert.AreEqual(2, GameState.Instance.unlockables[itemId]);

            prefab = GameObject.Instantiate(prefab);
            prefab.GetComponent<ItemInformation>().item = item;
            prefab.GetComponent<UnlockOnGrab>().unlockSound = null;
            prefab.GetComponent<UnlockOnGrab>().OnGrab(fakeGrabber);

            Assert.AreEqual(2, GameState.Instance.unlockables[itemId]);
        }

        [TearDown]
        public void Cleanup()
        {
            //Definitions.ItemsWithInformation = new Dictionary<string, HarvestDataTypes.Item>();
            GameState.Instance.unlockables = new Dictionary<string, int> { }; 
            Object.DestroyImmediate(databaseManager);

        }
        void StartDatabaseManager()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ScriptableObjects/Databases/DatabaseManager/DatabaseManager.prefab");
            databaseManager = GameObject.Instantiate(prefab);
            DatabaseManager.Instance = databaseManager.GetComponent<DatabaseManager>();
        }
    }
}