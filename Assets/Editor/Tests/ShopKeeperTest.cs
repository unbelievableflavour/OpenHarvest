using BNG;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class ShopKeeperTest
    {
        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/NPC's/Shopkeeper/ShopKeeper.prefab");
            prefab = GameObject.Instantiate(prefab);

            Assert.AreNotEqual(null, prefab.GetComponent<NPCController>().handSlot);
            Assert.AreNotEqual(null, prefab.GetComponent<NPCController>().NPCAnimator);

            var handSlot = prefab.transform.Find("HandSlot");
            Assert.AreNotEqual(null, handSlot.GetComponent<FollowTransform>().FollowTarget);
        }
    }
}
