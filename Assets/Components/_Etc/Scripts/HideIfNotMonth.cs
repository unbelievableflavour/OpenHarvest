using System;
using UnityEngine;

public class HideIfNotMonth : MonoBehaviour
{
    [Header("Format: MM, Example: 08")]
    public string monthToEnable = "08";

    void Start()
    {
        string sMonth = DateTime.Now.ToString("MM");

        if(sMonth != monthToEnable)
        {
            gameObject.SetActive(false);
            return;
        }
    }
}
