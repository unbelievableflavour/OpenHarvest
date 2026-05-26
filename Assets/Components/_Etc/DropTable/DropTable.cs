using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DroppedItem
{
    public int dropRate;
    public GameObject item;
}

public class DropTable : MonoBehaviour {
    public string description;

    [Header("Hover items for explanation. ")]
    [Header("Default drop table is used when empty")]
    [Tooltip("Drop rate explanation\n10 : copper\n20: silver\n5: gold\n\n10 + 20 + 5 = 35(alles bij elkaar)\n\nrandomNumber = random(0, 35)\n\nrandomNumber = 5: copper\nrandomNumber = 8: copper\nrandomNumber = 11: silver\nrandomNumber = 30: silver\nrandomNumber = 32: gold")]
    public List<DroppedItem> items = new List<DroppedItem>();

    public GameObject GetItemByDropRate()
    {        
        var randomNumber = Random.Range(0, getTotalDropRateCount());
        return GetItem(randomNumber);
    }

    public GameObject GetItem(int randomNumber)
    {
        int totalDropRateCount = 0;

        foreach (DroppedItem item in items)
        {
            totalDropRateCount += item.dropRate;

            if (randomNumber <= totalDropRateCount)
            {
                return item.item;
            }
        }

        return null;
    }

    public int getTotalDropRateCount()
    {
        int totalDropRateCount = 0;
        foreach(DroppedItem item in items)
        {
            totalDropRateCount += item.dropRate;
        }

        return totalDropRateCount;
    }
}