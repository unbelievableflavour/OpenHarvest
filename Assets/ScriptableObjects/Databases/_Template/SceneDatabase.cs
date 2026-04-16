using System;
using System.Collections.Generic;
using UnityEngine;

namespace HarvestDataTypes
{
    [CreateAssetMenu(fileName = "New Scene Database", menuName = "Harvest/Databases/Scenes")]
    public class SceneDatabase : ScriptableObject
    {
        public List<SceneSettings> scenes = new List<SceneSettings>();

        public SceneSettings FindByScene(string activeSceneName, int activeSceneBuildIndex)
        {
            if (scenes == null)
            {
                return null;
            }

            for (int i = 0; i < scenes.Count; i++)
            {
                SceneSettings entry = scenes[i];
                if (entry == null)
                {
                    continue;
                }

                bool buildIndexMatches = entry.sceneBuildIndex >= 0 && entry.sceneBuildIndex == activeSceneBuildIndex;
                bool nameMatches = !string.IsNullOrWhiteSpace(entry.sceneName) &&
                    string.Equals(entry.sceneName, activeSceneName, StringComparison.OrdinalIgnoreCase);

                if (buildIndexMatches || nameMatches)
                {
                    return entry;
                }
            }

            return null;
        }
    }
}
