#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;

namespace HarvestDataTypes
{
    [CustomEditor(typeof(Recipe))]
    public class RecipeEditor : DatabaseConnectedObjectEditor
    {
        private string dbLocation = "Assets/ScriptableObjects/Databases/RecipeDatabase.asset";

        public override void InitializeDB() {
            currentItem = (Recipe)target;
            db = (RecipeDatabase)AssetDatabase.LoadAssetAtPath(dbLocation, typeof(RecipeDatabase));
            dbList = db.recipes;
        }

        public void OnEnable()
        {
            base.OnEnable();
            
            currentItem.itemId = currentItem.name;
        }
    }
}
#endif