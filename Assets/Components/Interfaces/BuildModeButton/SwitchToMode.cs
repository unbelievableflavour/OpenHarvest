using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwitchToMode : MonoBehaviour
{
    public string mode;

    public void SwitchMode() {
        if(GameState.Instance.GetMode() != mode) {
            GameState.Instance.SwitchToMode(mode);
        } else {
            GameState.Instance.SwitchToMode("default");
        }
    }
}
