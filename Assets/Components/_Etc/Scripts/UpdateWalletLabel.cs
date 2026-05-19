using UnityEngine;
using UnityEngine.UI;

public class UpdateWalletLabel : MonoBehaviour
{
    Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

    public void UpdateMoneyLabel()
    {
        text.text = GameState.Instance.getTotalAmount().ToString();
    }
}