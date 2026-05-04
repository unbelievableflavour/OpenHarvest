using HarvestDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContractsOfTheWeekController : MonoBehaviour
{
    static System.Random rnd = new System.Random();

    public ContractDatabase contracts;
    public ContractsLister contractLister;
    public AutoType daysLeftIndicator;

    private int numberOfRandomContracts = 5;

    void Start()
    {
        TimeController.Instance.ListenToDayChange(handleNewDayStarted);

        if (getContractsOfTheWeek().currentContracts.Count == 0)
        {
            GameState.Instance.contractsOfTheWeek.currentContracts = getRandomContracts();
        }

        if (getContractsOfTheWeek().nextContracts.Count == 0)
        {
            GameState.Instance.contractsOfTheWeek.nextContracts = getRandomContracts();
        }

        Refresh();
    }

    private string GetDaysLeft()
    {
        int currentDayOfTheWeek = (int)TimeController.GetTotalSimulatedGameSeconds() / 86400 % 7;
        int daysLeft = 7 - currentDayOfTheWeek;
        return (daysLeft == 0 ? 7.ToString() : daysLeft.ToString());
    }

    private void handleNewDayStarted(object sender, EventArgs e)
    {
        Refresh();
    }

    private void Refresh()
    {
        UpdateContractsOfTheWeekIfNeeded();
        daysLeftIndicator.ResetText("Days before new contracts arrive: " + GetDaysLeft());
        contractLister.RefreshStore();
    }

    private void UpdateContractsOfTheWeekIfNeeded()
    {
        int currentWeek = (int)TimeController.GetTotalSimulatedGameSeconds() / 604800;
        if (currentWeek == getContractsOfTheWeek().week)
        {
            return;
        }

        SetContractsOfTheWeek(currentWeek);
    }

    void SetContractsOfTheWeek(int week)
    {
        getContractsOfTheWeek().week = week;
        getContractsOfTheWeek().currentContracts = getContractsOfTheWeek().nextContracts;

        bool continueTillNotDuplicate = true;
        while (continueTillNotDuplicate)
        {
            List<Contract> newNextItemOfTheWeek = getRandomContracts();
            if (getContractsOfTheWeek().nextContracts != newNextItemOfTheWeek)
            {
                GameState.Instance.contractsOfTheWeek.nextContracts = newNextItemOfTheWeek;
                break;
            }
        }
    }

    private ContractsOfTheWeek getContractsOfTheWeek()
    {
        return GameState.Instance.contractsOfTheWeek;
    }

    private List<Contract> getRandomContracts() 
    {
        return contracts.contracts.OrderBy(x => rnd.Next()).Take(numberOfRandomContracts).ToList();
    }

    private void OnDestroy()
    {
        TimeController.Instance.RemoveFromDayChange(handleNewDayStarted);
    }
}

