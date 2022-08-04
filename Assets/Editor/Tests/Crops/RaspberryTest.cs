using UnityEngine;
using NUnit.Framework;
using UnityEditor;

namespace Crops
{
    public class RaspberryTest
    {
        GameObject plant;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Crops/Raspberry/Raspberry.prefab");
            plant = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            Assert.AreNotEqual(null, plant.GetComponent<GrowthState>().healthIndicator);
            Assert.AreNotEqual(null, plant.GetComponent<GrowthState>().growthStates);
            Assert.AreNotEqual(null, plant.GetComponent<GrowthState>().objectSpawner);
            Assert.AreNotEqual(null, plant.GetComponent<GrowthState>().spawnManager);
            Assert.AreNotEqual(null, plant.GetComponent<ObjectSpawner>().spawnLocations);
            Assert.AreEqual(true, plant.GetComponent<ObjectSpawner>().spawnChangeInPercentage > 1);
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
        public void ItUsesADropTableForSpawning()
        {
            var objectSpawner = plant.GetComponentInChildren<ObjectSpawner>(true);
            var dropTable = objectSpawner.GetComponent<DropTable>();

            Assert.AreEqual(dropTable, objectSpawner.dropTable);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(plant);
        }
    }
}
