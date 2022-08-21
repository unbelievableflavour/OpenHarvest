using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableConsole : MonoBehaviour
{
    public HarvestSettings HarvestSettings;
    public GameObject debugMenu;

    void Start()
    {
        if (HarvestSettings.enableIngameConsole) {
            debugMenu.SetActive(true);
        }
    }
}
