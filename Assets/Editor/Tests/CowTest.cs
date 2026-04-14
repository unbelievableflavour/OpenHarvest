using BNG;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class CowTest
    {
        GameObject animal;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Animals/Cow/Adult/CowAnimated.prefab");
            animal = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            Assert.AreNotEqual(null, animal.GetComponent<AnimatedCow>().animator);
            Assert.AreNotEqual(0, animal.GetComponent<PetSound>().soundsToPlay.Length);

            var firstMesh = animal.GetComponentInChildren<SkinnedMeshRenderer>();
            Assert.AreNotEqual(null, firstMesh.sharedMaterials[0]);
            Assert.AreNotEqual(null, firstMesh.sharedMesh);

            var milkingArea = animal.GetComponentInChildren<MilkingAreaController>();
            Assert.AreNotEqual(null, milkingArea);
            Assert.AreNotEqual(null, milkingArea.tooltip);
            Assert.AreNotEqual(null, milkingArea.tooltipText);
            Assert.AreNotEqual(null, milkingArea.bucketSnapZone);
            Assert.AreNotEqual(null, milkingArea.doneVersion);

            var udders = animal.GetComponentsInChildren<Milkable>();
            Assert.AreEqual(4, udders.Length);

            foreach (Milkable milkable in udders)
            {
                Assert.AreNotEqual(null, milkable.milkingAreaController);
            }
        }

        [Test]
        public void ItTogglesTheIndicatorsOnAssistMode()
        {
            var assistObjectToggler = animal.GetComponentInChildren<AssistObjectToggler>();

            Assert.AreEqual("CanvasGroupIndicator", assistObjectToggler.objectToToggle.name);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(animal);
        }
    }
}
