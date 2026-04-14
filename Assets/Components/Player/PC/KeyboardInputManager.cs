using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardInputManager : MonoBehaviour
{
    private void Update() {
        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame){
            if(GameState.Instance.GetMode() == "default") {
                GameState.Instance.SwitchToMode("build");
            } else {
                GameState.Instance.SwitchToMode("default");
            }
        }
    }
}
