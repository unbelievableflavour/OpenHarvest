using UnityEngine;

public class RecipeRow : MonoBehaviour
{
    RecipeMenuController recipeMenuController;
    int recipeIndex;

    public void initialiseRecipeRow(RecipeMenuController recipeMenuController, int recipeIndex)
    {
        this.recipeMenuController = recipeMenuController;
        this.recipeIndex = recipeIndex;
    }
    public void SetActiveRecipeIndex()
    {
        recipeMenuController.SetDetailView(recipeIndex);
    }
}
