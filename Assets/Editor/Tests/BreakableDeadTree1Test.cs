using UnityEngine;
using NUnit.Framework;
using UnityEditor;

namespace Tests
{
    public class BreakableDeadTree1Test
    {
        GameObject breakable;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/BreakableTree/DeadTree/BreakableDeadTree1.prefab");
            breakable = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            var meshes = breakable.GetComponentsInChildren<MeshFilter>(true);

            foreach (MeshFilter mesh in meshes)
            {
                Assert.AreNotEqual(null, mesh.sharedMesh);
            }

            var meshRenderers = breakable.GetComponentsInChildren<MeshRenderer>(true);

            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                Assert.AreNotEqual(null, meshRenderer.sharedMaterial);
            }
        }


        [Test]
        public void ItCanBeChoppedDownWithATool()
        {
            var breakableObjectController = breakable.GetComponentInChildren<BreakableObjectController>();

            Assert.AreEqual(5, breakableObjectController.health);
            Assert.AreNotEqual(null, breakableObjectController.healthBar);
            Assert.AreEqual("Axe", breakableObjectController.toolUsedForBreaking);
            Assert.AreNotEqual(null, breakableObjectController.hitEffect);

            var objectSpawner = breakable.GetComponentInChildren<ObjectSpawner>(true);

            Assert.AreNotEqual(null, objectSpawner.spawnLocations);
            Assert.AreEqual(false, objectSpawner.useSpawnLocationAsParent);
            Assert.AreEqual(true, objectSpawner.rotateOnSpawn);
            Assert.AreEqual(true, objectSpawner.rotateOnSpawnZ);
            Assert.AreEqual(100, objectSpawner.spawnChangeInPercentage);
        }

        [Test]
        public void ItUsesCollisionForBreaking()
        {
            var breakableObjectController = breakable.GetComponentInChildren<BreakableObjectController>();

            Assert.AreEqual(true, breakableObjectController.useOnCollisionEnter);
        }

        [Test]
        public void ItUsesADropTableForSpawning()
        {
            var objectSpawner = breakable.GetComponentInChildren<ObjectSpawner>(true);
            var dropTable = objectSpawner.GetComponent<DropTable>();

            Assert.AreEqual(dropTable, objectSpawner.dropTable);
        }

        [Test]
        public void ItTogglesTheIndicatorsOnAssistMode()
        {
            var assistObjectToggler = breakable.GetComponentInChildren<AssistObjectToggler>();

            Assert.AreEqual("CanvasGroupIndicator", assistObjectToggler.objectToToggle.name);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(breakable);
        }
    }
}
