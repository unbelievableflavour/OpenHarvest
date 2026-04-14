using UnityEngine;

public class StackIncreaser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ItemStack>().IncreaseStack(99);
        GetComponent<ItemStack>().UpdateStackIndicator();
    }
}
