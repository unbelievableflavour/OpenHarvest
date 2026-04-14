using System;
using UnityEngine;

public class HideIfNotDate : MonoBehaviour
{
    [Header("Format: YYYY-MM-DD, Example: 2021-01-30")]

    public string from = "2018-12-16";
    public string till = "2021-12-16";

    private DateTime currentDate;

    private void Start()
    {
        DateTime fromDate = DateTime.Parse(from);
        DateTime tillDate = DateTime.Parse(till);

        currentDate = DateTime.Now;
        currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0);

        if(currentDate < fromDate || currentDate > tillDate)
        {
            gameObject.SetActive(false);
            return;
        }
    }
}
