using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Modules
{
    public class MushroomGreenShortTest
    {
        GameObject module;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/DungeonGenerator/Modules/Room/MushroomGreenShort/MushroomGreenShort.prefab");
            module = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItHasParentModuleForAllExits()
        {
            foreach(ModuleConnector moduleConnector in module.GetComponent<Module>().GetExits())
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