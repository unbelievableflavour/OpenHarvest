using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG
{
    public class PlayerHeightOffset : MonoBehaviour
    {
        BNGPlayerController player;
        void Start()
        {
            player = GetComponent<BNGPlayerController>();
            player.CharacterControllerYOffset = float.Parse(GameState.Instance.settings["playerHeightOffset"]);
        }
    }
}
