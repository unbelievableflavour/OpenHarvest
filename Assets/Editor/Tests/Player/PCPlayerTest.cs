using BNG;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Player
{
    public class PCPlayerTest
    {
        GameObject player;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/_Etc/Player/PC/PCController.prefab");
            Assert.IsNotNull(prefab, "PC player prefab not found at expected path.");
            player = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItChecksMatchesTheFieldOfViewOfTheQuest()
        {
            var fpsController = player.GetComponentInChildren<FirstPersonController>(true);
            Assert.IsNotNull(fpsController, "FirstPersonController component is missing on PC player prefab.");
            Assert.AreEqual(98, fpsController.fov);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(player);
        }
    }
}