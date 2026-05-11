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

        [Test]
        public void SpawnQuestReward_ReplacesHeldItemInHandSlot()
        {
            var npc = instantiatedPrefab.GetComponent<NPCController>();
            var snapZone = npc.handSlot.GetComponent<SnapZone>();
            Assert.IsNotNull(snapZone);

            var pieApple = AssetDatabase.LoadAssetAtPath<HarvestDataTypes.Item>("Assets/Items/PieApple/PieApple.asset");
            var hatMiner = AssetDatabase.LoadAssetAtPath<HarvestDataTypes.Item>("Assets/Items/HatMiner/HatMiner.asset");
            Assert.IsNotNull(pieApple);
            Assert.IsNotNull(hatMiner);
            Assert.IsNotNull(pieApple.prefab);
            Assert.IsNotNull(hatMiner.prefab);

            GameObject handedItem = Object.Instantiate(pieApple.prefab);
            snapZone.GrabGrabbable(handedItem.GetComponent<Grabbable>());
            Assert.IsNotNull(snapZone.HeldItem);

            npc.SpawnQuestReward(hatMiner);

            Assert.IsNotNull(snapZone.HeldItem);
            ItemInformation info = snapZone.HeldItem.GetComponent<ItemInformation>();
            Assert.IsNotNull(info);
            Assert.IsNotNull(info.item);
            Assert.AreEqual("HatMiner", info.item.itemId);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(instantiatedPrefab);
        }
    }
}
