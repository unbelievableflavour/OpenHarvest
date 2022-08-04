using System.Collections.Generic;
using UnityEngine;

namespace HarvestDataTypes
{

    [CreateAssetMenu(fileName = "New Contract Database", menuName = "Harvest/Databases/Contracts")]
    public class ContractDatabase : ScriptableObject
    {
        public List<Contract> contracts;
        public Contract FindById(string id)
        {
            return contracts.Find(contract => contract.contractId == id);
        }
    }
}

