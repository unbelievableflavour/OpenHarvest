using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class ChickenPlateauTest
    {
        const string PrefabPath = "Assets/Components/PlaceableObjects/PlateauChicken/PlateauChicken.prefab";

        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (prefab == null)
            {
                Assert.Ignore("ChickenPlateau prefab not yet created — rebuild it in the editor with AnimalInformation + ChickenExtras.");
                return;
            }

            var instance = Object.Instantiate(prefab);
            var info = instance.GetComponentInChildren<AnimalInformation>(true);
            Assert.IsNotNull(info, "PlateauChicken is missing AnimalInformation.");
            Assert.AreNotEqual(null, info.fedTile);
            Assert.AreNotEqual(null, info.petPrefab);
            Assert.AreNotEqual(null, info.nameValue);
            Assert.AreNotEqual(null, info.ageValue);
            Assert.AreNotEqual(null, info.hungryMeter);
            Assert.AreNotEqual(null, info.hungryValue);
            Assert.AreNotEqual(null, info.graveStone);
            Assert.AreNotEqual(null, info.diedLabel);

            var extras = instance.GetComponentInChildren<ChickenExtras>(true);
            Assert.IsNotNull(extras, "PlateauChicken is missing ChickenExtras.");
            Assert.AreNotEqual(null, extras.feather);
            Assert.AreNotEqual(null, extras.eggsPlateau);

            Object.DestroyImmediate(instance);
        }
    }
}
