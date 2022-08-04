using BNG;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class WindmillMechanicsTest
    {
        GameObject instantiatedPrefab;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/WindmillMechanics/WindmillMechanics.prefab");
            instantiatedPrefab = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            var meshes = instantiatedPrefab.GetComponentsInChildren<MeshFilter>(true);

            foreach (MeshFilter mesh in meshes)
            {
                Assert.AreNotEqual(null, mesh.sharedMesh);
            }

            var meshRenderers = instantiatedPrefab.GetComponentsInChildren<MeshRenderer>(true);

            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                Assert.AreNotEqual(null, meshRenderer.sharedMaterial);
            }
        }

        [Test]
        public void ItChecksIfAllRequiredFieldsForWindmillControllerAreNotEmpty()
        {
            Assert.AreNotEqual(null, instantiatedPrefab.GetComponentInChildren<WindmillController>().spawnManager);
            Assert.AreNotEqual(null, instantiatedPrefab.GetComponentInChildren<WindmillController>().wheatHeightController);
            Assert.AreNotEqual(null, instantiatedPrefab.GetComponentInChildren<WindmillController>().updateWheatCountLabel);
            Assert.AreNotEqual(null, instantiatedPrefab.GetComponentInChildren<WindmillController>().flourReadyLabel);
            Assert.AreNotEqual(null, instantiatedPrefab.GetComponentInChildren<WindmillController>().flourSnapZone);
        }

        [Test]
        public void ItChecksIfAllRequiredFieldsForWheatHeightControllerAreNotEmpty()
        {
            Assert.AreEqual(1, instantiatedPrefab.GetComponentsInChildren<WheatHeightController>().Length);
            Assert.AreNotEqual(null, instantiatedPrefab.GetComponentInChildren<WheatHeightController>().targetToMove);
            Assert.AreNotEqual(null, instantiatedPrefab.GetComponentInChildren<WheatHeightController>().full);
            Assert.AreNotEqual(null, instantiatedPrefab.GetComponentInChildren<WheatHeightController>().empty);
        }

        [Test]
        public void ItChecksIfAllRequiredFieldsForWindmillSpawnManagerAreNotEmpty()
        {
            Assert.AreEqual(1, instantiatedPrefab.GetComponentInChildren<SpawnManager>().daysUntilRespawn);
            Assert.AreNotEqual(null, instantiatedPrefab.GetComponentInChildren<SpawnManager>().timeIndicator);
            Assert.AreNotEqual(null, instantiatedPrefab.GetComponentInChildren<SpawnManager>().spawnEffect);
        }

        [Test]
        public void ItChecksIfAllRequiredFieldsForWindmillFlourButtonControllerAreNotEmpty()
        {
            Assert.AreNotEqual(null, instantiatedPrefab.GetComponentInChildren<WindmillFlourButtonController>().windmillController);
            Assert.AreNotEqual(null, instantiatedPrefab.GetComponentInChildren<WindmillFlourButtonController>().errorLabel);
            Assert.AreNotEqual(null, instantiatedPrefab.GetComponentInChildren<WindmillFlourButtonController>().errorMessage);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(instantiatedPrefab);
        }
    }
}
