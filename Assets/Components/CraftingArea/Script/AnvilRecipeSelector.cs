using UnityEngine;
using UnityEngine.UI;
using HarvestDataTypes;
using System.Collections.Generic;
using System.Linq;

public class AnvilRecipeSelector : MonoBehaviour
{
    public RecipeDatabase recipeDatabase;
    public CraftAreaController craftAreaController;
    public Transform recipeList;
    public GameObject recipeRow;

    private int currentRecipeIndex;

    private bool matchesCurrentIngredients(List<string> recipeIngredients, List<string> currentIngredients)
    {
        bool isEqual = recipeIngredients.OrderBy(x => x).SequenceEqual(currentIngredients.OrderBy(x => x));
 
        return isEqual;
    }

    public void RefreshListWithIngredients(List<string> ingredientIds)
    {
        foreach (Transform child in recipeList)
        {
            Destroy(child.gameObject);
        }

        var index = 0;
        foreach (Recipe recipe in recipeDatabase.getAllForMechanic(craftAreaController.mechanic))
        {
            if(!matchesCurrentIngredients(recipe.ingredients, ingredientIds)) {
                continue;
            }

            void buttonTask()
            {
                craftAreaController.SetActiveRecipe(recipe);
            }

            GameObject row = Instantiate(recipeRow);
            row.SetActive(true);
            var text = row.GetComponentInChildren<Text>();
            text.text = recipe.item.name;
            row.transform.SetParent(recipeList, false);

            var button = row.GetComponentInChildren<Button>();
            button.onClick.AddListener(buttonTask);
            button.enabled = true;
            index++;
        }
    }
}
