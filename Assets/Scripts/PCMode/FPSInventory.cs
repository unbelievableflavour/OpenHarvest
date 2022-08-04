using UnityEngine;

public class FPSInventory : MonoBehaviour
{
    public int currentItemIndex = 0;
    private GameObject currentItem;

    void Start()
    {
        currentItem = this.transform.GetChild(currentItemIndex).gameObject;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentItemIndex = 0;
            SetItemActive(currentItemIndex);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentItemIndex = 1;
            SetItemActive(currentItemIndex);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentItemIndex = 2;
            SetItemActive(currentItemIndex);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentItemIndex = 3;
            SetItemActive(currentItemIndex);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            currentItemIndex = 4;
            SetItemActive(currentItemIndex);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            currentItemIndex = 5;
            SetItemActive(currentItemIndex);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            currentItemIndex = 6;
            SetItemActive(currentItemIndex);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            currentItemIndex = 7;
            SetItemActive(currentItemIndex);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            currentItemIndex = 8;
            SetItemActive(currentItemIndex);
            return;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            currentItemIndex++;
            if (currentItemIndex >= this.transform.childCount) {
                currentItemIndex = this.transform.childCount;
                return;
            }

            SetItemActive(currentItemIndex);
            return;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            currentItemIndex--;

            if (currentItemIndex < 0)
            {
                currentItemIndex = 0;
                return;
            }

            SetItemActive(currentItemIndex);
            return;
        }
    }

    private void SetItemActive(int itemIndex)
    {
        currentItem.SetActive(false);
        currentItem = this.transform.GetChild(itemIndex).gameObject;
        currentItem.SetActive(true);
    }
}
