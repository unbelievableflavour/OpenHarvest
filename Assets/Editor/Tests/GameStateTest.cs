using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class GameStateTest
    {
        string itemId = "Greenhouse"; 
        GameObject databaseManager;

        [SetUp]
        public void SetUp()
        {
            Cleanup();
            StartDatabaseManager();

            var itemLoader = new ItemLoader();
            itemLoader.Start();
        }

        [Test]
        public void ItUnlocksTheItemOnce()
        {
            Assert.AreEqual(0, GameState.Instance.unlockables[itemId]);

            GameState.Instance.unlock(itemId, 1);

            Assert.AreEqual(1, GameState.Instance.unlockables[itemId]);
        }

        [Test]
        public void ItUnlocksTheItemTwice()
        {
            Assert.AreEqual(0, GameState.Instance.unlockables[itemId]);

            GameState.Instance.unlock(itemId, 2);

            Assert.AreEqual(2, GameState.Instance.unlockables[itemId]);
        }

        [Test]
        public void ItChecksIfAnItemIsUnlocked()
        {
            Assert.AreEqual(false, GameState.Instance.isUnlocked(itemId));

            GameState.Instance.unlock(itemId, 1);

            Assert.AreEqual(true, GameState.Instance.isUnlocked(itemId));
        }

        [Test]
        public void ItWillResetUnlockablesOnGameStateReset()
        {
            GameState.Instance.unlock("Windmill", 2);

            Assert.AreEqual(2, GameState.Instance.unlockables["Windmill"]);

            GameState.Reset();

            Assert.AreEqual(0, GameState.Instance.unlockables["Windmill"]);
        }

        [Test]
        public void ItCanAddAndDecreaseGold()
        {
            GameState.Instance.IncreaseMoneyByAmount(15);
            Assert.AreEqual(15, GameState.Instance.getTotalAmount());
            GameState.Instance.DecreaseMoneyByAmount(15);
            Assert.AreEqual(0, GameState.Instance.getTotalAmount());
        }

        [Test]
        public void ItCannotAddMoreGoldThenAnIntegerAllows()
        {
            GameState.Instance.IncreaseMoneyByAmount(Definitions.maxIntValue);
            GameState.Instance.IncreaseMoneyByAmount(Definitions.maxIntValue);
            GameState.Instance.IncreaseMoneyByAmount(Definitions.maxIntValue);
            Assert.AreEqual(Definitions.maxIntValue, GameState.Instance.getTotalAmount());
            GameState.Instance.DecreaseMoneyByAmount(Definitions.maxIntValue);
            Assert.AreEqual(0, GameState.Instance.getTotalAmount());
        }

        [TearDown]
        public void Cleanup()
        {
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