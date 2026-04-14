using BNG;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class SheepTest
    {
        GameObject animal;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Animals/Sheep/Adult/SheepAnimated.prefab");
            animal = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            Assert.AreNotEqual(null, animal.GetComponent<AnimatedSheep>().animator);
            Assert.AreNotEqual(0, animal.GetComponent<PetSound>().soundsToPlay.Length);

            var firstMesh = animal.GetComponentInChildren<SkinnedMeshRenderer>();
            Assert.AreNotEqual(null, firstMesh.sharedMaterials[0]);
            Assert.AreNotEqual(null, firstMesh.sharedMesh);

            Assert.AreNotEqual(null, animal.GetComponent<UniqueId>().uniqueId);
            Assert.AreNotEqual(null, animal.GetComponent<ShavingController>().spawnEffect);
            Assert.AreNotEqual(null, animal.GetComponent<ShavingController>().wool1);
            Assert.AreNotEqual(null, animal.GetComponent<ShavingController>().wool2);
            Assert.AreNotEqual(null, animal.GetComponent<ShavingController>().wool3);
            Assert.AreNotEqual(null, animal.GetComponent<ShavingController>().wool4);
        }

        [Test]
        public void ItUsesADropTableForSpawning()
        {
            var objectSpawners = animal.GetComponentsInChildren<ObjectSpawner>(true);
            var dropTable0 = objectSpawners[0].GetComponent<DropTable>();
            var dropTable1 = objectSpawners[1].GetComponent<DropTable>();
            var dropTable2 = objectSpawners[2].GetComponent<DropTable>();
            var dropTable3 = objectSpawners[3].GetComponent<DropTable>();

            Assert.AreEqual(dropTable0, objectSpawners[0].dropTable);
            Assert.AreEqual(dropTable1, objectSpawners[1].dropTable);
            Assert.AreEqual(dropTable2, objectSpawners[2].dropTable);
            Assert.AreEqual(dropTable3, objectSpawners[3].dropTable);
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
