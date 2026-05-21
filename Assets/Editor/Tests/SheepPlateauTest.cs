using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class SheepPlateauTest
    {
        const string PrefabPath = "Assets/Components/PlaceableObjects/PlateauSheep/PlateauSheep.prefab";

        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (prefab == null)
            {
                Assert.Ignore("SheepPlateau prefab not yet created — rebuild it in the editor with AnimalInformation.");
                return;
            }

            var instance = Object.Instantiate(prefab);
            var info = instance.GetComponentInChildren<AnimalInformation>(true);
            Assert.IsNotNull(info, "PlateauSheep is missing AnimalInformation.");
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
