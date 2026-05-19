using UnityEngine;
namespace HarvestDataTypes
{

    [CreateAssetMenu(fileName = "New RecipeMechanic", menuName = "Harvest/DataTypes/RecipeMechanic")]
    public class RecipeMechanic : ScriptableObject
    {
        public string name;
        public Sprite icon;
    }

}