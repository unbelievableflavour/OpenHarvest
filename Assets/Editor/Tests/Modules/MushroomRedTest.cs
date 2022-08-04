using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using static Definitions;

namespace Tests
{
    public class MushroomRedTest
    {
        GameObject module;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/DungeonGenerator/Modules/MushroomRed/MushroomRed.prefab");
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


        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(module);
        }
    }
}