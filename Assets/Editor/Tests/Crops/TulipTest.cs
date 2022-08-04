using UnityEngine;
using NUnit.Framework;
using UnityEditor;

namespace Crops
{
    public class TulipTest
    {
        GameObject plant;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Crops/Tulip/Tulip.prefab");
            plant = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            Assert.AreNotEqual(null, plant.GetComponent<GrowthState>().healthIndicator);
            Assert.AreNotEqual(null, plant.GetComponent<GrowthState>().growthStates);
            Assert.AreEqual(null, plant.GetComponent<GrowthState>().objectSpawner);
            Assert.AreNotEqual(null, plant.GetComponent<GrowthState>().spawnManager);
            Assert.AreNotEqual(null, plant.GetComponent<SpawnManager>().timeIndicator);
            Assert.AreNotEqual(null, plant.GetComponent<SpawnManager>().spawnEffect);
            Assert.AreNotEqual(null, plant.GetComponent<SpawnManager>().behaviourObject);

            var meshes = plant.GetComponentsInChildren<MeshFilter>(true);

            foreach (MeshFilter mesh in meshes)
            {
                Assert.AreNotEqual(null, mesh.sharedMesh);
            }

            var meshRenderers = plant.GetComponentsInChildren<MeshRenderer>(true);

            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                Assert.AreNotEqual(null, meshRenderer.sharedMaterial);
            }
        }

        [Test]
        public void ItCanBeChoppedDownWithATool()
        {
            var breakableObjectController = plant.GetComponentInChildren<BreakableObjectController>(true);

            Assert.AreEqual(2, breakableObjectController.health);
            Assert.AreNotEqual(null, breakableObjectController.healthBar);
            Assert.AreEqual("Sickle", breakableObjectController.toolUsedForBreaking);
            Assert.AreNotEqual(null, breakableObjectController.hitEffect);

            var objectSpawner = plant.GetComponentInChildren<ObjectSpawner>(true);

            Assert.AreEqual(false, objectSpawner.useSpawnLocationAsParent);
            Assert.AreEqual(true, objectSpawner.rotateOnSpawn);
            Assert.AreEqual(false, objectSpawner.rotateOnSpawnZ);
            Assert.AreEqual(100, objectSpawner.spawnChangeInPercentage);
        }

        [Test]
        public void ItUsesCollisionForBreaking()
        {
            var breakableObjectController = plant.GetComponentInChildren<BreakableObjectController>(true);

            Assert.AreEqual(true, breakableObjectController.useOnCollisionEnter);
        }

        [Test]
        public void ItUsesADropTableForSpawning()
        {
            var objectSpawner = plant.GetComponentInChildren<ObjectSpawner>(true);
            var dropTable = objectSpawner.GetComponent<DropTable>();

            Assert.AreEqual(dropTable, objectSpawner.dropTable);
        }

        [Test]
        public void ItTogglesTheIndicatorsOnAssistMode()
        {
            var assistObjectToggler = plant.GetComponentInChildren<AssistObjectToggler>(true);

            Assert.AreEqual("CanvasGroupIndicator", assistObjectToggler.objectToToggle.name);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(plant);
        }
    }
}
