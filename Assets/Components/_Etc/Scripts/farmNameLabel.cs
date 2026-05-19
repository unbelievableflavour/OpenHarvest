using UnityEngine;
using UnityEngine.UI;

public class farmNameLabel : MonoBehaviour
{
    void Start()
    {
        GetComponent<Text>().text = GameState.Instance.farmName;
    }
}
