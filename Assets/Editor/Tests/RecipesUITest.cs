using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class RecipesUITest
    {
        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Components/_Etc/Interfaces/RecipesUI/RecipeUI.prefab");

            prefab = GameObject.Instantiate(prefab);

            Assert.AreNotEqual(null, prefab.GetComponent<RecipeMenuController>().viewSwitcher);
            Assert.AreNotEqual(null, prefab.GetComponent<RecipeMenuController>().recipeList);
            Assert.AreNotEqual(null, prefab.GetComponent<RecipeMenuController>().recipeRow);
            Assert.AreNotEqual(null, prefab.GetComponent<RecipeMenuController>().detailsHeader);
            Assert.AreNotEqual(0, prefab.GetComponent<RecipeMenuController>().GetRecipesCount());
        }
    }
}

