using UnityEngine;
using UnityEngine.UI;
using HarvestDataTypes;
using System.Collections.Generic;

public class AnvilRecipeSelector : MonoBehaviour
{
    public RecipeDatabase recipeDatabase;
    public CraftAreaController craftAreaController;
    public Transform recipeList;
    public GameObject recipeRow;

    private int currentRecipeIndex;

    public void RefreshListWithIngredient(HarvestDataTypes.Item item)
    {
        foreach (Transform child in recipeList)
        {
            Destroy(child.gameObject);
        }

        var index = 0;
        foreach (Recipe recipe in recipeDatabase.getAllForMechanic(craftAreaController.mechanic))
        {
            if(recipe.ingredients[0] != item.itemId) {
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
