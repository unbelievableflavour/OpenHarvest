using System.Collections.Generic;
using UnityEngine;
namespace HarvestDataTypes
{
    public enum OnFail
    {
        spawnFailItem,
        keepSuccessItem,
    };

    [CreateAssetMenu(fileName = "New Recipe", menuName = "Harvest/DataTypes/Recipe")]
    public class Recipe : ScriptableObject
    {
        [ReadOnly]
        public string itemId;
        public Item item;
        public OnFail onFail;
        public Item failedItem;
        public List<string> ingredients;
        public string toolId;
        public RecipeMechanic mechanic;
    }
}