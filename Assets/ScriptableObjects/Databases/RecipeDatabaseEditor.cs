#if (UNITY_EDITOR) 
using UnityEngine;
using UnityEditor;
using HarvestDataTypes;

[CustomEditor(typeof(RecipeDatabase))]
public class RecipeDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var recipeDatabase = (RecipeDatabase)target;

        if (GUILayout.Button("Add all recipes", GUILayout.Height(20)))
        {
            foreach (string s in AssetDatabase.FindAssets("", new string[] { "Assets/ScriptableObjects/Recipes/" }))
            {
                Recipe recipe = (Recipe)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(s), typeof(Recipe));

                if (recipe == null)
                {
                    continue;
                }

                if (recipeDatabase.recipes.Contains(recipe))
                {
                    continue;
                }
                recipeDatabase.recipes.Add(recipe);
                EditorUtility.SetDirty(recipeDatabase);
            }
        }
    }
}
#endif