using System.Collections.Generic;
using UnityEngine;

namespace HarvestDataTypes
{
    [CreateAssetMenu(fileName = "New Item Database", menuName = "Harvest/Databases/Items")]
    public class ItemDatabase : ScriptableObject
    {
        public List<Item> items;
        public Item fallbackItem;

        public Item FindById(string id)
        {
            return items.Find(item => item.itemId == id);
        }

        public List<Item> FindAllByTag(string tag)
        {
            return items.FindAll(item => item.tags.Contains(tag));
        }
    }
}

