using UnityEngine;
using NUnit.Framework;
using UnityEditor;
using BNG;

namespace Tests
{
    public class CookingAreaOutDoorTest
    {
        GameObject outdoorCookingArea;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/CookingAreaOutDoor/CookingAreaOutDoor.prefab");
            outdoorCookingArea = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            Assert.AreNotEqual(null, outdoorCookingArea.GetComponent<DisableWhenFarAway>().maxShowDistance);
            Assert.AreNotEqual(null, outdoorCookingArea.GetComponent<DisableWhenFarAway>().gameObjectToDisable);

            var meshes = outdoorCookingArea.GetComponentsInChildren<MeshFilter>(true);

            foreach (MeshFilter mesh in meshes)
            {
                Assert.AreNotEqual(null, mesh.sharedMesh);
            }

            var meshRenderers = outdoorCookingArea.GetComponentsInChildren<MeshRenderer>(true);

            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                Assert.AreNotEqual(null, meshRenderer.sharedMaterial);
            }
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(outdoorCookingArea);
        }
    }
}
