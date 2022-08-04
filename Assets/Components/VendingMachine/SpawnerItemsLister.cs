using UnityEngine;
using UnityEngine.UI;
using static Definitions;

public class SpawnerItemsLister : MonoBehaviour
{
    public GameObject itemRowPrefab;
    public Transform functionalScrollViewContent;
    public SpawnController spawnController;

    void Start()
    {
        fillSpawnerList();
    }

    private void fillSpawnerList()
    {
        foreach (string itemId in Definitions.itemStores["vendingMachine"])
        {
            if (GameState.isUnlockable(itemId) && !GameState.isUnlocked(itemId))
            {
                continue;
            }

            Item itemInfo = GetItemInformation(itemId);
            if (itemInfo == null)
            {
                continue;
            }

            GameObject row = Instantiate(itemRowPrefab);
            row.SetActive(true);
            var text = row.GetComponentInChildren<Text>();
            text.text = itemInfo.name;

            var spawnSelector = row.GetComponentInChildren<SpawnerSelector>();
            spawnSelector.SetItem(itemId);
            spawnSelector.SetSpawnController(spawnController);
            row.transform.SetParent(functionalScrollViewContent, false);
        }
    }
}
