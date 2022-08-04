using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Modules
{
    public class VeinGoldTest
    {
        GameObject module;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/DungeonGenerator/Modules/VeinGold/VeinGold.prefab");
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
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            Assert.AreEqual(0, module.GetComponent<Module>().Colliders.Length);

            var meshes = module.GetComponentsInChildren<MeshFilter>(true);

            foreach (MeshFilter mesh in meshes)
            {
                Assert.AreNotEqual(null, mesh.sharedMesh);
            }

            var meshRenderers = module.GetComponentsInChildren<MeshRenderer>(true);

            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                Assert.AreNotEqual(null, meshRenderer.sharedMaterial);
            }
        }

        [Test]
        public void ItChecksIfBreakableObjectControllerIsConfiguredCorrectly()
        {
            var breakableObjectController = module.GetComponentInChildren<BreakableObjectController>();
            Assert.AreEqual("Pickaxe", breakableObjectController.toolUsedForBreaking);
            Assert.AreEqual(8, breakableObjectController.health);
            Assert.AreNotEqual(null, breakableObjectController.healthBar);
            Assert.AreNotEqual(null, breakableObjectController.hitEffect);
            Assert.AreEqual(true, breakableObjectController.useOnCollisionEnter);
            Assert.AreEqual(true, breakableObjectController.useCooldownCollisionZone);
        }

        [Test]
        public void ItChecksIfSpawningIsConfiguredCorrectly()
        {
            var objectSpawner = module.GetComponentInChildren<ObjectSpawner>();
            var dropTable = module.GetComponentInChildren<DropTable>();
            Assert.AreEqual(true, objectSpawner.rotateOnSpawn);
            Assert.AreEqual(false, objectSpawner.rotateOnSpawnZ);
            Assert.AreNotEqual(null, objectSpawner.spawnLocations);

            Assert.AreEqual(dropTable, objectSpawner.dropTable);
            Assert.AreNotEqual(0, dropTable.items.Count);

            foreach (DroppedItem droppedItem in dropTable.items)
            {
                Assert.AreNotEqual(0, droppedItem.dropRate);
                Assert.AreNotEqual(null, droppedItem.item);
            }
        }

        [Test]
        public void ItTogglesTheIndicatorsOnAssistMode()
        {
            var assistObjectToggler = module.GetComponentInChildren<AssistObjectToggler>();

            Assert.AreEqual("CanvasGroupIndicator", assistObjectToggler.objectToToggle.name);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(module);
        }
    }
}