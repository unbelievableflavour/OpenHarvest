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
        string resourcePath = "Assets/Items/HatChicken/HatChicken.prefab";
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
        }

        [Test]
        public void ItUnlocksTheItemOnGrabbing()
        {
            Assert.AreEqual(false, GameState.Instance.unlockables.ContainsKey(itemId));

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
            Assert.AreEqual(false, GameState.Instance.unlockables.ContainsKey(itemId));

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
            item.maximumTimesOwned = 2;

            Assert.AreEqual(false, GameState.Instance.unlockables.ContainsKey(itemId));

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
            GameState.Reset();
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