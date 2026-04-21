#if (UNITY_EDITOR)
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HarvestDataTypes
{
    [CustomEditor(typeof(SceneDatabase))]
    public class SceneDatabaseEditor : Editor
    {
        private static readonly string[] IncludedFolders = new string[]
        {
            "Assets/Scenes/"
        };

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var database = (SceneDatabase)target;

            if (GUILayout.Button("Add all scene settings", GUILayout.Height(20)))
            {
                database.scenes = new List<SceneSettings>();
                var buildIndexBySceneName = BuildIndexBySceneName();

                foreach (string guid in AssetDatabase.FindAssets("t:SceneSettings", IncludedFolders))
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    SceneSettings sceneSettings = (SceneSettings)AssetDatabase.LoadAssetAtPath(
                        assetPath,
                        typeof(SceneSettings)
                    );

                    if (sceneSettings == null)
                    {
                        continue;
                    }

                    string parentFolderName = GetParentFolderName(assetPath);
                    sceneSettings.sceneName = parentFolderName;
                    sceneSettings.sceneBuildIndex = buildIndexBySceneName.TryGetValue(parentFolderName, out int buildIndex)
                        ? buildIndex
                        : -1;

                    database.scenes.Add(sceneSettings);
                    EditorUtility.SetDirty(sceneSettings);
                }

                EditorUtility.SetDirty(database);
            }
        }

        private static Dictionary<string, int> BuildIndexBySceneName()
        {
            var buildIndexBySceneName = new Dictionary<string, int>();
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                string sceneName = Path.GetFileNameWithoutExtension(scenes[i].path);
                if (string.IsNullOrWhiteSpace(sceneName))
                {
                    continue;
                }

                buildIndexBySceneName[sceneName] = i;
            }

            return buildIndexBySceneName;
        }

        private static string GetParentFolderName(string assetPath)
        {
            string parentDirectory = Path.GetDirectoryName(assetPath);
            if (string.IsNullOrWhiteSpace(parentDirectory))
            {
                return string.Empty;
            }

            return Path.GetFileName(parentDirectory.Replace("\\", "/"));
        }
    }
}
#endif
