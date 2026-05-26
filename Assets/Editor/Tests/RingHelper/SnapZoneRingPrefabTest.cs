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
        const string SnapZoneRingPrefabPath = "Assets/Components/_Etc/RingHelper/SnapZoneRing/SnapZoneRing.prefab";
        const string RingHelperPrefabPath = "Assets/Components/_Etc/RingHelper/RingHelper.prefab";

        GameObject instance;
        string prefabPath;

        [SetUp]
        public void SetUp()
        {
            prefabPath = SnapZoneRingPrefabPath;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            Assert.IsNotNull(prefab, "Snap zone ring prefab missing at " + prefabPath);
            instance = Object.Instantiate(prefab);
        }

        [Test]
        public void RingHelper_QuadUsesBillboardRingShader()
        {
            prefabPath = RingHelperPrefabPath;
            Object.DestroyImmediate(instance);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            Assert.IsNotNull(prefab, "RingHelper prefab missing at " + prefabPath);
            instance = Object.Instantiate(prefab);

            MeshRenderer renderer = instance.GetComponentInChildren<MeshRenderer>(true);
            Assert.IsNotNull(renderer, "RingHelper prefab must have a MeshRenderer on its quad child.");

            Material material = renderer.sharedMaterial;
            Assert.IsNotNull(material, "RingHelper quad must use RingHelperMaterial.");
            Assert.IsNotNull(material.shader, "RingHelper material must reference a shader.");
            Assert.AreEqual(
                "Custom/BillboardRing",
                material.shader.name,
                "Grabbable RingHelper must use Custom/BillboardRing (URP) on RingHelperMaterial.");
            Assert.Greater(
                material.GetFloat("_WorldHalfExtent"),
                0f,
                "Ring size must be driven by shader _WorldHalfExtent, not transform scale.");
        }

        [Test]
        public void SnapZoneRing_HasNoBillboardRingHelper()
        {
            Assert.AreEqual(
                0,
                instance.GetComponentsInChildren<BillboardRingHelper>(true).Length,
                "Snap zone rings must not use BillboardRingHelper (hands-full logic hides backpack slots).");
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

        [Test]
        public void SnapZoneRing_QuadUsesFlatRingShader()
        {
            MeshRenderer renderer = instance.GetComponentInChildren<MeshRenderer>(true);
            Assert.IsNotNull(renderer, "SnapZoneRing prefab must have a MeshRenderer on its quad child.");

            Material material = renderer.sharedMaterial;
            Assert.IsNotNull(material, "SnapZoneRing quad must use SnapZoneRing material.");
            Assert.IsNotNull(material.shader, "SnapZoneRing material must reference a shader.");
            Assert.AreEqual(
                "Custom/FlatRing",
                material.shader.name,
                "Snap zone rings must use Custom/FlatRing (URP), not BillboardRing.");
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
