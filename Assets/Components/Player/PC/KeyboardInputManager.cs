using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputManager : MonoBehaviour
{
    private void Update() {
        if(Input.GetKeyDown(KeyCode.B)){
            if(GameState.Instance.GetMode() == "default") {
                GameState.Instance.SwitchToMode("build");
            } else {
                GameState.Instance.SwitchToMode("default");
            }
        }
    }
}
