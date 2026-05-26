using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class LanternWithRopeTest
    {
        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/_Etc/Lantern/LanternWithRope/LanternWithRope.prefab");
            prefab = GameObject.Instantiate(prefab);

            Assert.AreNotEqual(null, prefab.GetComponent<NightLight>().light);
            Assert.AreNotEqual(null, prefab.GetComponent<NightLight>().lanternLightMesh);
        }
    }
}
