using HarvestDataTypes;
using UnityEngine;
using UnityEngine.UI;

public class ContractRow : MonoBehaviour
{
    public Text buttonLabel;
    public Button button;

    private Contract contract;
    private ContractsLister parentLister;

    public void SetContract(Contract contract)
    {
        this.contract = contract;
        RefreshButton();
    }

    private void RefreshButton()
    {
        button.interactable = true;
    }

    public void GoToDetailPage()
    {
        parentLister.detailPage.SetContract(contract);
        parentLister.viewSwitcher.setActiveView("detail");
    }

    public void Refresh()
    {
        SetContract(contract);
    }

    public void SetParentLister(ContractsLister parentLister)
    {
        this.parentLister = parentLister;
    }
}
