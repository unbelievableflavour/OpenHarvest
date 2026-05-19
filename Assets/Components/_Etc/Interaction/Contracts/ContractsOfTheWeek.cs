using HarvestDataTypes;
using System;
using System.Collections.Generic;

[Serializable]
public class SaveableContract
{
    public string contractId = "";
}

[Serializable]
public class SaveableContractsOfTheWeek
{
    public int week = 0;

    public List<SaveableContract> currentContracts;
    public List<SaveableContract> nextContracts;
}

public class ContractsOfTheWeek
{
    public int week = 0;

    public List<Contract> currentContracts;
    public List<Contract> nextContracts;

    public SaveableContractsOfTheWeek ToSaveable()
    {
        SaveableContractsOfTheWeek saveableContractsOfTheWeek = new SaveableContractsOfTheWeek();
        saveableContractsOfTheWeek.week = week;
        saveableContractsOfTheWeek.currentContracts = new List<SaveableContract>();
        saveableContractsOfTheWeek.nextContracts = new List<SaveableContract>();

        foreach (Contract contract in currentContracts)
        {
            saveableContractsOfTheWeek.currentContracts.Add(new SaveableContract() { contractId = contract.contractId });
        }

        foreach (Contract contract in nextContracts)
        {
            saveableContractsOfTheWeek.nextContracts.Add(new SaveableContract() { contractId = contract.contractId });
        }

        return saveableContractsOfTheWeek;
    }
    public void ImportSaveable(SaveableContractsOfTheWeek saveableContractsOfTheWeek)
    {
        week = saveableContractsOfTheWeek.week;
        currentContracts = new List<Contract>();
        nextContracts = new List<Contract>();

        foreach (SaveableContract contract in saveableContractsOfTheWeek.currentContracts)
        {
            currentContracts.Add(DatabaseManager.Instance.contracts.FindById(contract.contractId));
        }

        foreach (SaveableContract contract in saveableContractsOfTheWeek.nextContracts)
        {
            nextContracts.Add(DatabaseManager.Instance.contracts.FindById(contract.contractId));
        }
    }
}
