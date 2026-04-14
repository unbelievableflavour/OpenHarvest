using BNG;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class DoorPortalTest
    {
        GameObject portal;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/DoorPortal/DoorPortal.prefab");
            portal = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItShouldNotCollideWithPlayerTeleporting()
        {
            Assert.AreEqual(LayerMask.NameToLayer("Dont Collide With Teleport"), portal.layer);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(portal);
        }
    }
}
