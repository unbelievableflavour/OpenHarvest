using System.Collections.Generic;
using UnityEngine;

namespace HarvestDataTypes
{
    [CreateAssetMenu(fileName = "New Placeable Object Database", menuName = "Harvest/Databases/Placeable Objects")]
    public class PlaceableObjectDatabase : ScriptableObject
    {
        public List<PlaceableObject> objectsData;

        public PlaceableObject FindById(string id)
        {
            return objectsData.Find(item => item.placeableObjectId == id);
        }
    }
}
