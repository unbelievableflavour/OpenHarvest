using UnityEngine;
using NUnit.Framework;
using UnityEditor;

namespace Tests
{
    public class BreakableStoneTest
    {
        GameObject breakable;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/PatchBreakables/BreakableStone.prefab");
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

            Assert.AreEqual(3, breakableObjectController.health);
            Assert.AreNotEqual(null, breakableObjectController.healthBar);
            Assert.AreEqual("Hammer", breakableObjectController.toolUsedForBreaking);
            Assert.AreNotEqual(null, breakableObjectController.hitEffect);
        }

        [Test]
        public void ItUsesCollisionForBreaking()
        {
            var breakableObjectController = breakable.GetComponentInChildren<BreakableObjectController>();

            Assert.AreEqual(true, breakableObjectController.useOnCollisionEnter);
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
