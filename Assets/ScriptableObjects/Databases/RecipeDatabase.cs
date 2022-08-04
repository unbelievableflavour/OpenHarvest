using System.Collections.Generic;
using UnityEngine;

namespace HarvestDataTypes {

    [CreateAssetMenu(fileName = "New Recipe Database", menuName = "Harvest/Databases/Recipes")]
    public class RecipeDatabase : ScriptableObject
    {
        public List<Recipe> recipes;

        public List<Recipe> getAllForMechanic(RecipeMechanic mechanic)
        {
            return recipes.FindAll(recipe => recipe.mechanic == mechanic);
        }
    }

}

