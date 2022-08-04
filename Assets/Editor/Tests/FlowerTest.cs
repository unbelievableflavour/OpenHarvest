using UnityEngine;
using NUnit.Framework;
using UnityEditor;

namespace Tests
{
    public class FlowerTest
    {
        GameObject gameObject;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Flower/Flower.prefab");
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
            Assert.AreEqual(5, gameObject.GetComponent<LODGroup>().GetLODs()[0].renderers.Length);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(gameObject);
        }
    }
}

