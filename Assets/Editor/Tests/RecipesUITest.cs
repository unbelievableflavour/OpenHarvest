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
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Interfaces/RecipesUI/RecipeMenu.prefab");
            prefab = GameObject.Instantiate(prefab);

            Assert.AreNotEqual(null, prefab.GetComponent<RecipeMenuController>().listView);
            Assert.AreNotEqual(null, prefab.GetComponent<RecipeMenuController>().recipeList);
            Assert.AreNotEqual(null, prefab.GetComponent<RecipeMenuController>().detailsView);
            Assert.AreNotEqual(null, prefab.GetComponent<RecipeMenuController>().recipeRow);
            Assert.AreNotEqual(null, prefab.GetComponent<RecipeMenuController>().detailsHeader);
            //Assert.AreNotEqual(null, prefab.GetComponent<RecipeMenuController>().detailsDescription);
            Assert.AreNotEqual(0, prefab.GetComponent<RecipeMenuController>().GetRecipesCount());
        }
    }
}

