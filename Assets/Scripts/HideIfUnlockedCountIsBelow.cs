﻿using UnityEngine;
using static Definitions;

public class HideIfUnlockedCountIsBelow : MonoBehaviour
{
    public string itemType;
    public int unlockedCount = 1;

    void Start()
    {
        if (GameState.unlockables.ContainsKey(itemType) == false) 
        {
            this.gameObject.SetActive(false);
            return;
        }

        if (GameState.unlockables[itemType] < unlockedCount)
        {
            this.gameObject.SetActive(false);
            return;
        }
    }
}
