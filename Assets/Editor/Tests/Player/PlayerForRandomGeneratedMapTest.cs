using BNG;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Player
{
    public class PlayerForRandomGeneratedMapTest
    {
        GameObject player;
        Transform xrRigAdvanced;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Player/PlayerForRandomGeneratedMap.prefab");
            player = GameObject.Instantiate(prefab);
            xrRigAdvanced = player.transform.Find("XR Rig Advanced");
        }

        [Test]
        public void ItHasElevationCheckerDisabled()
        {
            var elevationChecker = xrRigAdvanced.GetComponent<ElevationChecker>();

            Assert.AreEqual(false, elevationChecker.enabled);
        }

        [TearDown]
        public void Cleanup()
        {
            xrRigAdvanced = null;
            Object.DestroyImmediate(player);
        }
    }
}