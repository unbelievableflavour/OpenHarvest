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
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Player/NewCustomPlayerAdvanced Variant.prefab");
            player = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItChecksMatchesTheFieldOfViewOfTheQuest()
        {
            var fpsController = player.GetComponentInChildren<FirstPersonController>(true);
            Assert.AreEqual(98, fpsController.GetComponentInChildren<FirstPersonController>(true).fov);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(player);
        }
    }
}