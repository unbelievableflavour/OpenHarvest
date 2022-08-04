using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemStack : MonoBehaviour
{
    public Text stackindicator;
    private int currentStackSize = 1; 
    private string uniqueId;

    public ItemStack()
    {
        uniqueId = Guid.NewGuid().ToString();
    }

    public void IncreaseStack(int size)
    {
        currentStackSize = currentStackSize + size;
        UpdateStackIndicator();
    }

    public void DecreaseStack(int size)
    {
        currentStackSize = currentStackSize - size;
        UpdateStackIndicator();
    }

    public int GetStackSize()
    {
        return currentStackSize;
    }

    public void SetStackSize(int size)
    {
        currentStackSize = size;
        UpdateStackIndicator();
    }

    public void UpdateStackIndicator()
    {
        stackindicator.gameObject.SetActive(currentStackSize > 1);
        stackindicator.text = currentStackSize.ToString();
    }

    public string GetUniqueId()
    {
        return uniqueId;
    }
}
