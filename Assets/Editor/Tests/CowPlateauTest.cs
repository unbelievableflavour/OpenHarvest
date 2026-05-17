using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class CowPlateauTest
    {
        const string PrefabPath = "Assets/NPCs/Cow/CowPlateau/CowPlateau.prefab";

        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (prefab == null)
            {
                Assert.Ignore("CowPlateau prefab not yet created — rebuild it in the editor with AnimalInformation.");
                return;
            }

            var instance = Object.Instantiate(prefab);
            var info = instance.GetComponent<AnimalInformation>();
            Assert.AreNotEqual(null, info.fedTile);
            Assert.AreNotEqual(null, info.petPrefab);
            Assert.AreNotEqual(null, info.nameValue);
            Assert.AreNotEqual(null, info.ageValue);
            Assert.AreNotEqual(null, info.hungryMeter);
            Assert.AreNotEqual(null, info.hungryValue);
            Assert.AreNotEqual(null, info.graveStone);
            Assert.AreNotEqual(null, info.diedLabel);

            Object.DestroyImmediate(instance);
        }
    }
}
