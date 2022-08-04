using BNG;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class SheepPlateauTest
    {
        [Test]
        public void ItChecksIfAllRequiredFielsdAreNotEmpty()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Animals/Sheep/SheepPlateau/SheepPlateau.prefab");
            prefab = GameObject.Instantiate(prefab);

            Assert.AreNotEqual(null, prefab.GetComponent<AnimalInformation>().fedTile);
            Assert.AreNotEqual(null, prefab.GetComponent<AnimalInformation>().petYoung);
            Assert.AreNotEqual(null, prefab.GetComponent<AnimalInformation>().pet);
            Assert.AreNotEqual(null, prefab.GetComponent<AnimalInformation>().nameValue);
            Assert.AreNotEqual(null, prefab.GetComponent<AnimalInformation>().ageValue);
            Assert.AreNotEqual(null, prefab.GetComponent<AnimalInformation>().hungryMeter);
            Assert.AreNotEqual(null, prefab.GetComponent<AnimalInformation>().hungryValue);
            Assert.AreNotEqual(null, prefab.GetComponent<AnimalInformation>().graveStone);
            Assert.AreNotEqual(null, prefab.GetComponent<AnimalInformation>().diedLabel);
        }
    }
}
