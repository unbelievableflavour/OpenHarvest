using UnityEngine;
using NUnit.Framework;
using UnityEditor;

namespace Tests
{
    public class GreenhouseTest
    {
        GameObject gameObject;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/GreenHouse/GreenHouse.prefab");
            gameObject = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            var meshes = gameObject.GetComponentsInChildren<MeshFilter>(true);

            foreach (MeshFilter mesh in meshes)
            {
                Assert.AreNotEqual(null, mesh.sharedMesh);
            }

            var meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>(true);

            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                Assert.AreNotEqual(null, meshRenderer.sharedMaterial);
            }
        }

        [Test]
        public void ItChecksIfTheLODsAreConfigured()
        {
            Assert.AreNotEqual(null, gameObject.GetComponent<LODGroup>());
            Assert.AreEqual(12, gameObject.GetComponent<LODGroup>().GetLODs()[0].renderers.Length);
            Assert.AreEqual(4, gameObject.GetComponent<LODGroup>().GetLODs()[1].renderers.Length);
        }

        [Test]
        public void ItChecksIfTheDoorHasTheAParamForTheSoilGrid()
        {
            var switchSceneOnDoorOpen = gameObject.GetComponentInChildren<SwitchSceneOnDoorOpen>(true);

            Assert.AreEqual("greenhouse1", switchSceneOnDoorOpen.sceneEnterLocationName);

        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(gameObject);
        }
    }
}
