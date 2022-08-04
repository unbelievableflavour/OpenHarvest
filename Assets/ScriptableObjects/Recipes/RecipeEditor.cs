#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
namespace HarvestDataTypes
{
    [CustomEditor(typeof(Recipe))]
    public class RecipeEditor : Editor
    {
        private string dbLocation = "Assets/ScriptableObjects/Databases/RecipeDatabase.asset";

        RecipeDatabase db;
        private bool currentToggleValue;
        private bool lastToggleValue;

        public void OnEnable()
        {
            var recipe = (Recipe)target;

            recipe.itemId = recipe.name;

            db = (RecipeDatabase)AssetDatabase.LoadAssetAtPath(dbLocation, typeof(RecipeDatabase));
            currentToggleValue = db.recipes.Contains(recipe);
            lastToggleValue = currentToggleValue;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var recipe = (Recipe)target;

            currentToggleValue = GUILayout.Toggle(currentToggleValue, "Added to Database", GUILayout.Height(20));
            if (lastToggleValue != currentToggleValue)
            {
                if (currentToggleValue)
                {
                    db.recipes.Add(recipe);
                    EditorUtility.SetDirty(db);
                }
                else
                {
                    db.recipes.Remove(recipe);
                    EditorUtility.SetDirty(db);
                }

                lastToggleValue = currentToggleValue;
            }
        }
    }
}
#endif