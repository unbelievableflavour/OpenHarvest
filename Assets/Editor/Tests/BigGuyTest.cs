using BNG;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class BigGuyTest
    {
        GameObject instantiatedPrefab;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/NPC's/BigGuy/BigGuy.prefab");
            instantiatedPrefab = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            Assert.AreNotEqual(null, instantiatedPrefab.GetComponent<NPCController>().handSlot);
            Assert.AreNotEqual(null, instantiatedPrefab.GetComponent<NPCController>().NPCAnimator);

            var handSlot = instantiatedPrefab.transform.Find("HandSlot");
            Assert.AreNotEqual(null, handSlot.GetComponent<FollowTransform>().FollowTarget);
        }

        [Test]
        public void BoundingCollidersHaveTheAppropriateTag() //DontCollideWithTeleport (all layers are actually fine except Default)
        {
            Assert.AreEqual(1, instantiatedPrefab.GetComponentsInChildren<SphereCollider>().Length);
            Assert.AreEqual(20, instantiatedPrefab.GetComponentInChildren<SphereCollider>().gameObject.layer);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(instantiatedPrefab);
        }
    }
}
