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
            Assert.AreEqual(0, GameState.unlockables[itemId]);

            GameState.unlock(itemId, 1);

            Assert.AreEqual(1, GameState.unlockables[itemId]);
        }

        [Test]
        public void ItUnlocksTheItemTwice()
        {
            Assert.AreEqual(0, GameState.unlockables[itemId]);

            GameState.unlock(itemId, 2);

            Assert.AreEqual(2, GameState.unlockables[itemId]);
        }

        [Test]
        public void ItChecksIfAnItemIsUnlocked()
        {
            Assert.AreEqual(false, GameState.isUnlocked(itemId));

            GameState.unlock(itemId, 1);

            Assert.AreEqual(true, GameState.isUnlocked(itemId));
        }

        [Test]
        public void ItWillResetUnlockablesOnGameStateReset()
        {
            GameState.unlock("Windmill", 2);

            Assert.AreEqual(2, GameState.unlockables["Windmill"]);

            GameState.Reset();

            Assert.AreEqual(0, GameState.unlockables["Windmill"]);
        }

        [Test]
        public void ItCanAddAndDecreaseGold()
        {
            GameState.IncreaseMoneyByAmount(15);
            Assert.AreEqual(15, GameState.getTotalAmount());
            GameState.DecreaseMoneyByAmount(15);
            Assert.AreEqual(0, GameState.getTotalAmount());
        }

        [Test]
        public void ItCannotAddMoreGoldThenAnIntegerAllows()
        {
            GameState.IncreaseMoneyByAmount(Definitions.maxIntValue);
            GameState.IncreaseMoneyByAmount(Definitions.maxIntValue);
            GameState.IncreaseMoneyByAmount(Definitions.maxIntValue);
            Assert.AreEqual(Definitions.maxIntValue, GameState.getTotalAmount());
            GameState.DecreaseMoneyByAmount(Definitions.maxIntValue);
            Assert.AreEqual(0, GameState.getTotalAmount());
        }

        [TearDown]
        public void Cleanup()
        {
            GameState.unlockables = new Dictionary<string, int> { };
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