using UnityEngine;
using UnityEngine.UI;
using HarvestDataTypes;
using System.Collections.Generic;

public class SpawnerItemsLister : MonoBehaviour
{
    public GameObject itemRowPrefab;
    public Transform functionalScrollViewContent;
    public SpawnController spawnController;

    List<HarvestDataTypes.Item> itemsInVendingMachine = new List<HarvestDataTypes.Item>();

    void Start()
    {
        var itemDatabase = DatabaseManager.Instance.items;
        itemsInVendingMachine = itemDatabase.FindAllByTag("vendingMachine");
        fillSpawnerList();
    }

    private void fillSpawnerList()
    {
        foreach (HarvestDataTypes.Item item in itemsInVendingMachine)
        {
            if (item == null)
            {
                continue;
            }

            if (item.isUnlockable && !GameState.Instance.isUnlocked(item.itemId))
            {
                continue;
            }

            GameObject row = Instantiate(itemRowPrefab);
            row.SetActive(true);
            var text = row.GetComponentInChildren<Text>();
            text.text = item.name;

            var spawnSelector = row.GetComponentInChildren<SpawnerSelector>();
            spawnSelector.SetItem(item);
            spawnSelector.SetSpawnController(spawnController);
            row.transform.SetParent(functionalScrollViewContent, false);
        }
    }
}
