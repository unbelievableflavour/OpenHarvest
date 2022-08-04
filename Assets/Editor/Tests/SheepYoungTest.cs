using BNG;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class SheepYoungTest
    {
        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Animals/Sheep/Young/SheepYoungAnimated.prefab");
            prefab = GameObject.Instantiate(prefab);

            Assert.AreNotEqual(null, prefab.GetComponent<AnimatedSheep>().animator);
            Assert.AreNotEqual(0, prefab.GetComponent<PetSound>().soundsToPlay.Length);

            var firstMesh = prefab.GetComponentInChildren<SkinnedMeshRenderer>();
            Assert.AreNotEqual(null, firstMesh.sharedMaterials[0]);
            Assert.AreNotEqual(null, firstMesh.sharedMesh);
        }
    }
}
