using UnityEngine;
using UnityEngine.UI;
using HarvestDataTypes;
using System.Collections.Generic;

public class RecipeMenuController : MonoBehaviour
{
    public RecipeDatabase recipeDatabase;
    public GameObject listView;
    public Transform recipeList;
    public GameObject detailsView;
    public GameObject recipeRow;

    public Text detailsHeader;
    public Text detailsIngredientsValue;
    public Text detailsToolValue;
    public Image detailsRecipeMechanicImage;
    public Text detailsRecipeMechanicValue;
    public Text detailsResultValue;

    private int currentRecipeIndex;
    private string view = "list";

    public void ActivateRecipeUI()
    {
        view = "list";
        RefreshView();
    }

    public void RefreshView()
    {
        if (view == "list") {
            listView.SetActive(true);
            detailsView.SetActive(false);
            fillRecipeList();
            return;
        }

        if (view == "detail")
        {
            detailsView.SetActive(true);
            listView.SetActive(false);

            var recipe = recipeDatabase.recipes[currentRecipeIndex];
            string ingredientsString = string.Join(" + ", recipe.ingredients);

            detailsHeader.text = recipe.item.name;
            detailsIngredientsValue.text = ingredientsString;
            detailsToolValue.text = recipe.toolId;
            detailsRecipeMechanicImage.sprite = recipe.mechanic.icon;
            detailsRecipeMechanicValue.text = recipe.mechanic.name;
            detailsResultValue.text = recipe.item.name;

            return;
        }
    }

    private void fillRecipeList()
    {
        foreach (Transform child in recipeList)
        {
            Destroy(child.gameObject);
        }

        var index = 0;
        foreach (Recipe recipe in recipeDatabase.recipes)
        {
            GameObject row = Instantiate(recipeRow);
            row.SetActive(true);
            var text = row.GetComponentInChildren<Text>();
            text.text = recipe.item.name;
            row.transform.SetParent(recipeList, false);

            row.GetComponent<RecipeRow>().initialiseRecipeRow(this, index);
            index++;
        }
    }

    public void SetDetailView(int newDetailViewIndex)
    {
        view = "detail";
        currentRecipeIndex = newDetailViewIndex;
        RefreshView();
    }

    public int GetRecipesCount()
    {
        return recipeDatabase.recipes.Count;
    }
}
