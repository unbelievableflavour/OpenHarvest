using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class LanternWithPoleTest
    {
        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Lantern/LanternWithPole/LanternWithPole.prefab");
            prefab = GameObject.Instantiate(prefab);

            Assert.AreNotEqual(null, prefab.GetComponent<NightLight>().light);
            Assert.AreNotEqual(null, prefab.GetComponent<NightLight>().lanternLightMesh);
        }
    }
}
