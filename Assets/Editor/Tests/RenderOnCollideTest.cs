using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using static Definitions;

namespace Tests
{
    public class RenderOnCollideTest
    {
        GameObject module;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/RenderOnCollide/RenderOnCollide.prefab");
            module = GameObject.Instantiate(prefab);
        }

        [Test]
        public void BoundingCollidersHaveTheAppropriateTag() //DontCollideWithTeleport (all layers are actually fine except Default)
        {
            Assert.AreEqual(20, module.gameObject.layer);
            Assert.AreNotEqual(null, module.GetComponent<BoxCollider>());
        }


        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(module);
        }
    }
}