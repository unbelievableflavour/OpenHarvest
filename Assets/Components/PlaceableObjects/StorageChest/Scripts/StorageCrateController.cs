using UnityEngine;
using UnityEngine.UI;

public class StorageCrateController : MonoBehaviour
{
    public Transform inventory;
    public Text currentPageLabel;

    int numberOfSlotsPerPage = 10;
    int currentIndex = 0;
    int currentPage = 1;
    int totalNumberOfSlots;

    void Start()
    {
        totalNumberOfSlots = inventory.childCount;
        UpdateCurrentPageLabel();
    }

    public void PreviousPage()
    {
        if (currentIndex == 0)
        {
            currentPage = totalNumberOfSlots / numberOfSlotsPerPage;
            currentIndex = totalNumberOfSlots - numberOfSlotsPerPage;
            RefreshPage();
            UpdateCurrentPageLabel();
            return;
        }
        currentPage -= 1;
        currentIndex -= numberOfSlotsPerPage;
        RefreshPage();
        UpdateCurrentPageLabel();
    }

    public void NextPage()
    {
        if(currentIndex+numberOfSlotsPerPage == totalNumberOfSlots)
        {
            currentPage = 1;
            currentIndex = 0;
            RefreshPage();
            UpdateCurrentPageLabel();
            return;
        }
        currentPage += 1;
        currentIndex += numberOfSlotsPerPage;
        RefreshPage();
        UpdateCurrentPageLabel();
    }

    void UpdateCurrentPageLabel()
    {
        currentPageLabel.text = currentPage + "/" + (totalNumberOfSlots / numberOfSlotsPerPage);
    }

    void DisableAll()
    {
        foreach (Transform child in inventory) {
            child.gameObject.SetActive(false);
        }
    }

    void RefreshPage()
    {
        DisableAll();
        for (int i = currentIndex; i < currentIndex + numberOfSlotsPerPage; i++)
        {
            inventory.GetChild(i).gameObject.SetActive(true);
        }
    }
}
