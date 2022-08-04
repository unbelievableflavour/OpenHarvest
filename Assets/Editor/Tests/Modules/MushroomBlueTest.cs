using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Modules
{
    public class MushroomBlueTest
    {
        GameObject module;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/DungeonGenerator/Modules/MushroomBlue/MushroomBlue.prefab");
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