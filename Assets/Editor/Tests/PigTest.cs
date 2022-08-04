using BNG;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class PigTest
    {
        GameObject animal;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Animals/Pig/Adult/PigAnimated.prefab");
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
        }

        [Test]
        public void ItUsesADropTableForSpawning()
        {
            var objectSpawner = animal.GetComponentInChildren<ObjectSpawner>(true);
            var dropTable = objectSpawner.GetComponent<DropTable>();

            Assert.AreEqual(dropTable, objectSpawner.dropTable);
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
