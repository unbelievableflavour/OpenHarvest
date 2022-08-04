using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using static Definitions;

namespace Tests
{
    public class Room2Test
    {
        GameObject module;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/DungeonGenerator/Modules/Room2/Room2.prefab");
            module = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItHasParentModuleForAllExits()
        {
            foreach (ModuleConnector moduleConnector in module.GetComponent<Module>().GetExits())
            {
                Assert.AreNotEqual(null, moduleConnector.parentModule);
            }
        }

        [Test]
        public void BoundingCollidersHaveTheAppropriateTag() //UnpreparedModuleCollider
        {
            Assert.AreNotEqual(0, module.GetComponent<Module>().Colliders.Length);

            foreach (Collider collider in module.GetComponent<Module>().Colliders)
            {
                Assert.AreEqual(19, collider.gameObject.layer);
            }
        }


        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(module);
        }
    }
}