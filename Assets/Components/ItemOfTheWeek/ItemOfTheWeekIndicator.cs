using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemOfTheWeekIndicator : MonoBehaviour
{
    public string itemOfTheWeekId = "plant";
    public Text indicator;
    public Text daysLeftIndicator;
    public Text newItemIndicator;
    private List<string> itemList;

    void Start()
    {
        itemList = getItemListByType();

        TimeController.Instance.ListenToDayChange(handleNewDayStarted);
        UpdateItemOfTheWeekIfNeeded();
    
        indicator.text = getItemOfTheWeekByType().currentItemId;
        daysLeftIndicator.text = "Days left: " + GetDaysLeft();
        newItemIndicator.text = "Next item: " + getItemOfTheWeekByType().nextItemId;
    }

    private string GetDaysLeft()
    {
        int currentDayOfTheWeek = (int)TimeController.GetTotalSimulatedGameSeconds() / 86400 % 7;
        int daysLeft = 7 - currentDayOfTheWeek;
        return (daysLeft == 0 ? 7.ToString() : daysLeft.ToString());
    }

    private void handleNewDayStarted(object sender, EventArgs e)
    {
        UpdateItemOfTheWeekIfNeeded();

        indicator.text = getItemOfTheWeekByType().currentItemId;
        daysLeftIndicator.text = "Days left: " + GetDaysLeft();
        newItemIndicator.text = "Next item: " + getItemOfTheWeekByType().nextItemId;
    }

    private void UpdateItemOfTheWeekIfNeeded()
    {
        int currentWeek = (int)TimeController.GetTotalSimulatedGameSeconds() / 604800;
        if (currentWeek == getItemOfTheWeekByType().week)
        {
            return;
        }

        SetItemOfTheWeek(currentWeek);
    }

    void SetItemOfTheWeek(int week)
    {
        getItemOfTheWeekByType().week = week;
        getItemOfTheWeekByType().currentItemId = getItemOfTheWeekByType().nextItemId;

        bool continueTillNotDuplicate = true;
        while (continueTillNotDuplicate)
        {
            var newNextItemOfTheWeek = itemList[UnityEngine.Random.Range(0, itemList.Count)];
            if (getItemOfTheWeekByType().nextItemId != newNextItemOfTheWeek)
            {
                GameState.Instance.itemsOfTheWeek[itemOfTheWeekId].nextItemId = newNextItemOfTheWeek;
                break;
            }
        }
    }

    private ItemOfTheWeek getItemOfTheWeekByType()
    {
        return GameState.Instance.itemsOfTheWeek[itemOfTheWeekId];
    }

    private List<string> getItemListByType()
    {
        return Definitions.itemsWithTypes[itemOfTheWeekId];
    }
}

