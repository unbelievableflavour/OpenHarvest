using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class StreetLampTest
    {
        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Decorations/StreetLamp/Street_lamp.prefab");
            prefab = GameObject.Instantiate(prefab);
            
            Assert.AreNotEqual(null, prefab.GetComponent<NightLight>().light);
            Assert.AreNotEqual(null, prefab.GetComponent<NightLight>().lanternLightMesh);
        }
    }
}
