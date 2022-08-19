using HarvestDataTypes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContractsLister : MonoBehaviour
{
    public ViewSwitcher viewSwitcher;
    public ContractsDetailPage detailPage;
    public GameObject contractRowPrefab;
    public Transform scrollViewContent;

    private void fillStore(List<Contract> contracts)
    {
        ClearStore();

        foreach (Contract item in contracts)
        {
            GameObject row = Instantiate(contractRowPrefab);
            row.SetActive(true);
            var text = row.GetComponentInChildren<Text>();
            text.text = item.name;
            row.transform.SetParent(scrollViewContent, false);

            var itemRow = row.GetComponentInChildren<ContractRow>();
            itemRow.SetContract(item);
            itemRow.SetParentLister(this);
        }
    }

    public void RefreshStore()
    {
        fillStore(GameState.Instance.contractsOfTheWeek.currentContracts);
    }

    public void RefreshStoreRows()
    {
        foreach (Transform item in scrollViewContent)
        {
            var itemRow = item.GetComponentInChildren<ItemRow>();
            itemRow.Refresh();
        }
    }

    public void LockStore(HarvestDataTypes.Item currentBoughtItem)
    {
        foreach (Transform item in scrollViewContent)
        {
            var itemRow = item.GetComponentInChildren<ItemRow>();
            itemRow.LockStoreItem(currentBoughtItem);
        }
    }

    public void UnlockStore()
    {
        foreach (Transform item in scrollViewContent)
        {
            var itemRow = item.GetComponentInChildren<ItemRow>();
            itemRow.Refresh();
        }
    }

    private void ClearStore()
    {
        foreach (Transform child in scrollViewContent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
