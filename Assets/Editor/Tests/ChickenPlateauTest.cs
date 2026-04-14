using BNG;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class ChickenPlateauTest
    {
        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Animals/Chicken/ChickenPlateau/ChickenPlateau.prefab");
            prefab = GameObject.Instantiate(prefab);

            Assert.AreNotEqual(null, prefab.GetComponent<ChickenInformation>().fedTile);
            Assert.AreNotEqual(null, prefab.GetComponent<ChickenInformation>().petYoung);
            Assert.AreNotEqual(null, prefab.GetComponent<ChickenInformation>().pet);
            Assert.AreNotEqual(null, prefab.GetComponent<ChickenInformation>().nameValue);
            Assert.AreNotEqual(null, prefab.GetComponent<ChickenInformation>().ageValue);
            Assert.AreNotEqual(null, prefab.GetComponent<ChickenInformation>().hungryMeter);
            Assert.AreNotEqual(null, prefab.GetComponent<ChickenInformation>().hungryValue);
            Assert.AreNotEqual(null, prefab.GetComponent<ChickenInformation>().graveStone);
            Assert.AreNotEqual(null, prefab.GetComponent<ChickenInformation>().diedLabel);
            Assert.AreNotEqual(null, prefab.GetComponent<ChickenInformation>().feather);
            Assert.AreNotEqual(null, prefab.GetComponent<ChickenInformation>().eggsPlateau);
        }
    }
}
