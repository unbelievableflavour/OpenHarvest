﻿using UnityEngine;

public class DeleteIfNotUnlocked : MonoBehaviour
{
    public string itemId;
    
    void Start()
    {
        if (GameState.isUnlocked(itemId))
        {
            return;
        }

        Destroy(this.gameObject);
    }
}
