using UnityEngine;
using NUnit.Framework;
using UnityEditor;

namespace Tests
{
    public class CanvasGroupIndicatorTest
    {
        GameObject canvasGroup;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/CanvasGroupIndicator/CanvasGroupIndicator.prefab");
            canvasGroup = GameObject.Instantiate(prefab);
        }

        [Test]
        public void ItCanvasGroupValues()
        {
            Assert.AreEqual(1, canvasGroup.GetComponentInChildren<CanvasGroup>().alpha);
        }

        [Test]
        public void ItMirrorsTheContent() //Hack till I find the culprit.
        {
            Assert.AreEqual(-1f, canvasGroup.transform.localScale.x);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(canvasGroup);
        }
    }
}