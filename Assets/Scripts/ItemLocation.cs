using UnityEngine;
using UnityEngine.UI;
using static Definitions;


public class ItemLocation : MonoBehaviour
{
    public ItemLocations itemLocation;
    public Transform options; 
    public GameObject chooserUI;
    public Text selectedItemLabel;
    public Text savedItemLabel;
    public Text currentNumberLabel;
    public Button saveButton;
    public Text saveButtonLabel;
    public Text ownedLabel;
    public Text usedLabel;

    private int activeIndex = 0;
    private ConfigurationModeToggler configurationModeToggler;

    void Start()
    {
        // It's a dirty hack I know.
        SetFakeGreenhouseLocations();

        SetTotalItemsNumber();
        chooserUI.SetActive(false);
        Load();
    }

    public int GetActiveIndex()
    {
        return activeIndex;
    }

    public void SetActiveIndex(int newIndex)
    {
        options.GetChild(activeIndex).gameObject.SetActive(false);
        activeIndex = newIndex;
        options.GetChild(activeIndex).gameObject.SetActive(true);
        UpdateLabels();
    }

    public void SetTotalItemsNumber()
    {
        int totalItemsNumber = 0;
        int currentItemFakeIndex = 1;
        int currentItemActualIndex = 0;

        foreach (Transform option in options)
        {
            if (currentItemActualIndex == activeIndex)
            {
                currentItemFakeIndex = totalItemsNumber + 1;
            }

            var currentItem = option.GetComponent<ItemInformation>();

            if (currentItem != null && !GameState.isUnlocked(currentItem.getItemId()))
            {
                currentItemActualIndex++;
                continue;
            }

            totalItemsNumber++;
            currentItemActualIndex++;
        }

        currentNumberLabel.text = currentItemFakeIndex + "/" + totalItemsNumber;
    }

    public void Next() {
        if (activeIndex < options.childCount - 1)
        {
            int i = 0;
            foreach (Transform option in options)
            {
                if(activeIndex >= i)
                {
                    i++;
                    continue;
                }

                var currentItem = option.GetComponent<ItemInformation>();
                if (currentItem != null && !GameState.isUnlocked(currentItem.getItemId()))
                {
                    i++;
                    continue;
                }
                SetActiveIndex(i);
                return;
            }
        }

        SetActiveIndex(0);
    }

    public void Previous()
    {
        if (activeIndex > 0)
        {
            for (int i = options.childCount - 1; i >= 0;)
            {
                if (activeIndex <= i)
                {
                    i--;
                    continue;
                }
                var currentItem = options.GetChild(i).GetComponent<ItemInformation>();
                if (currentItem != null && !GameState.isUnlocked(currentItem.getItemId()))
                {
                    i--;
                    continue;
                }

                SetActiveIndex(i);
                return;
            }
        }

        for (int i = options.childCount - 1; i >= 0;)
        {
            var currentItem = options.GetChild(i).GetComponent<ItemInformation>();
            if (currentItem != null && !GameState.isUnlocked(currentItem.getItemId()))
            {
                i--;
                continue;
            }

            SetActiveIndex(i);
            return;
        }
    }

    public void Load()
    {
        string currentSavedItem = GameState.locationConfigurations[itemLocation.ToString()];

        if(string.IsNullOrEmpty(currentSavedItem))
        {
            SetActiveIndex(0);
            return;
        }

        int i = 0;
        foreach (Transform option in options)
        {
            var currentItem = option.GetComponent<ItemInformation>();
            if (currentItem != null && currentSavedItem == currentItem.getItemId())
            {
                SetActiveIndex(i);
                return;
            }
            i++;
        }
    }

    public void Save()
    {
        var currentItem = options.GetChild(activeIndex).GetComponent<ItemInformation>();
        if (currentItem == null) {
            savedItemLabel.text = "Saved: none";
            GameState.locationConfigurations[itemLocation.ToString()] = null;
            return;
        }

        savedItemLabel.text = "Saved: " + currentItem.name;
        GameState.locationConfigurations[itemLocation.ToString()] = currentItem.getItemId();
        configurationModeToggler.RefreshItems();
    }

    public int GetCurrentlyUsedItemCount(string itemId)
    {
        int currentlyUsedCount = 0;

        foreach (var location in GameState.locationConfigurations)
        {
            if (location.Value == itemId)
            {
                currentlyUsedCount++;
            }
        }

        return currentlyUsedCount;
    }

    public void UpdateLabels()
    {
        saveButton.interactable = true;
        saveButtonLabel.text = "Save";
        saveButtonLabel.fontSize = 27;
        ownedLabel.text = "Owned: ∞";
        usedLabel.text = "Used: 0";

        var currentItem = options.GetChild(activeIndex).GetComponent<ItemInformation>();
        if (currentItem == null)
        {
            selectedItemLabel.text = "Current: None";
            SetTotalItemsNumber();
            return;
        }
        Item itemInfo = currentItem.getItemInfo();
        if (itemInfo == null)
        {
            selectedItemLabel.text = "Current: None";
            SetTotalItemsNumber();
            return;
        }

        selectedItemLabel.text = "Current: " + itemInfo.name;
        SetTotalItemsNumber();

        int currentlyOwnedCount = GameState.unlockables[currentItem.getItemId()];
        int currentlyUsedCount = GetCurrentlyUsedItemCount(currentItem.getItemId());

        ownedLabel.text = "Owned: " + currentlyOwnedCount.ToString();
        usedLabel.text = "Used: " + currentlyUsedCount.ToString();

        if (itemInfo.itemId == GameState.locationConfigurations[itemLocation.ToString()])
        {
            saveButton.interactable = false;
            saveButtonLabel.fontSize = 20;
            saveButtonLabel.text = "Currently used";
            return;
        }

        if (!(currentlyOwnedCount > currentlyUsedCount))
        {
            saveButton.interactable = false;
            saveButtonLabel.fontSize = 20;
            saveButtonLabel.text = "Buy more or remove elsewhere";
        }
    }

    public void SetConfigurationModeToggler(ConfigurationModeToggler newConfigurationModeToggler)
    {
        configurationModeToggler = newConfigurationModeToggler;
    }

    private void SetFakeGreenhouseLocations()
    {
        if (itemLocation == ItemLocations.GreenHouseSprinkler1)
        {
            if (GameState.enteredSceneThrough == "greenhouse1"){
                itemLocation = ItemLocations.GreenHouseSprinkler1;
                return;
            }
            if (GameState.enteredSceneThrough == "greenhouse2")
            {
                itemLocation = ItemLocations.GreenHouseSprinkler3;
                return;
            }
            if (GameState.enteredSceneThrough == "greenhouse3")
            {
                itemLocation = ItemLocations.GreenHouseSprinkler5;
                return;
            }
            return;
        }

        if (itemLocation == ItemLocations.GreenHouseSprinkler2)
        {
            if (GameState.enteredSceneThrough == "greenhouse1")
            {
                itemLocation = ItemLocations.GreenHouseSprinkler2;
                return;
            }
            if (GameState.enteredSceneThrough == "greenhouse2")
            {
                itemLocation = ItemLocations.GreenHouseSprinkler4;
                return;
            }
            if (GameState.enteredSceneThrough == "greenhouse3")
            {
                itemLocation = ItemLocations.GreenHouseSprinkler6;
                return;
            }
            return;
        }
    }
}
