using System;
using System.Collections.Generic;
using UnityEngine;
namespace HarvestDataTypes
{
    [Serializable]
    public class Requirement
    {
        public int amount = 0;
        public Item item;
    }

    [CreateAssetMenu(fileName = "New Contract", menuName = "Harvest/DataTypes/Contract")]
    public class Contract : ScriptableObject
    {
        [ReadOnly]
        public string contractId;
        public new string name;
        [TextArea]
        public string description;
        public List<Requirement> requirements;

        [Header("Rewards")]
        public int rewardGold;
    }

}