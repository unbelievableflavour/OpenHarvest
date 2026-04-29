using UnityEngine;

namespace HarvestDataTypes
{
    [CreateAssetMenu(fileName = "SceneSettings", menuName = "Harvest/Scene/Scene Settings")]
    public class SceneSettings : ScriptableObject
    {
        [Header("Scene Match")]
        [Tooltip("Scene name this configuration should apply to.")]
        public string sceneName = "";

        [Tooltip("Optional scene build index match. Use -1 to ignore build index.")]
        public int sceneBuildIndex = -1;

        [Header("Placeable Seeding")]
        [Tooltip("If enabled, seed owned amounts for placeables when the placement system starts.")]
        public bool seedPlaceableObjectsOnStart = false;

        [Tooltip("Owned amount assigned to each placeable during seeding.")]
        [Min(0)]
        public int seedPlaceableObjectCount = 10;

        [Header("Build Mode")]
        [Tooltip("If disabled, hide build mode entry points in scene UI.")]
        public bool allowBuildMode = true;

        [Header("Weather")]
        [Tooltip("If enabled, the WeatherController drives weather particles in this scene.")]
        public bool usesWeather = false;
    }
}
