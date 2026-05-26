using BNG;
using HarvestDataTypes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingBase : MonoBehaviour
{
    public GameObject tooltip;
    public Text tooltipText;

    protected List<Recipe> recipes;

    protected void RemoveOldItems(CookingTool cookingTool)
    {
        foreach (SnapZone snapZone in cookingTool.snapZones)
        {
            var oldGrabItem = snapZone.HeldItem;
            if (oldGrabItem != null)
            {
                Destroy(oldGrabItem.gameObject);
            }   
        }
    }

    protected Recipe GetRecipeFromCookingTool(CookingTool cookingTool)
    {
        List<string> ingredientsInPan = new List<string>();

        foreach (SnapZone snapZone in cookingTool.snapZones)
        {
            if (!snapZone.HeldItem)
            {
                continue;
            }

            if (snapZone.HeldItem.GetComponent<ItemStack>() && snapZone.HeldItem.GetComponent<ItemStack>().GetStackSize() > 1)
            {
                tooltip.SetActive(true);
                tooltipText.text = "Dont use item stacks while cooking";
                return null;
            }
            
            var item = Definitions.GetItemFromObject(snapZone.HeldItem);
            ingredientsInPan.Add(item.itemId);
        }

        if (ingredientsInPan.Count == 0)
        {
            tooltip.SetActive(true);
            tooltipText.text = "Put ingredients in first";
            return null;
        }

        foreach (Recipe recipe in recipes)
        {
            var set1 = new HashSet<string>(ingredientsInPan);
            var set2 = new HashSet<string>(recipe.ingredients);
            if (cookingTool.toolId != recipe.toolId)
            {
                continue;
            }

            if (!set1.SetEquals(set2))
            {
                continue;
            }
            return recipe;
        }

        tooltip.SetActive(true);
        tooltipText.text = "Not a valid recipe";
        return null;
    }
}
