using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Tests
{
    /// <summary>
    /// Snap zone rings must stay mesh + shader; world Canvas + Text per slot was a major perf cost.
    /// </summary>
    public class SnapZoneRingPrefabTest
    {
        const string PrefabPath = "Assets/Components/_Etc/RingHelper/SnapZoneRing/SnapZoneRing.prefab";

        GameObject instance;

        [SetUp]
        public void SetUp()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            Assert.IsNotNull(prefab, "SnapZoneRing prefab missing at " + PrefabPath);
            instance = Object.Instantiate(prefab);
        }

        [Test]
        public void SnapZoneRing_HasNoWorldUiCanvasStack()
        {
            Canvas[] canvases = instance.GetComponentsInChildren<Canvas>(true);
            Assert.AreEqual(0, canvases.Length,
                "SnapZoneRing must not include Canvas — use MeshRenderer + ring shader for performance.");

            CanvasScaler[] scalers = instance.GetComponentsInChildren<CanvasScaler>(true);
            Assert.AreEqual(0, scalers.Length,
                "SnapZoneRing must not include CanvasScaler — avoid world UI stack on snap markers.");

            Text[] texts = instance.GetComponentsInChildren<Text>(true);
            Assert.AreEqual(0, texts.Length,
                "SnapZoneRing must not include legacy Text (e.g. letter O) — use mesh ring only.");
        }

        [TearDown]
        public void TearDown()
        {
            if (instance != null)
            {
                Object.DestroyImmediate(instance);
            }
        }
    }
}
