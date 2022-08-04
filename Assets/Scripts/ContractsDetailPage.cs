using HarvestDataTypes;
using UnityEngine;
using UnityEngine.UI;

public class ContractsDetailPage : MonoBehaviour
{
    public AutoType heading;
    public Text description;
    public Text requirements;
    public Text rewards;
    public ContractSlotsController slotsController;

    private Contract currentContract;

    private void UpdateInformation(Contract contract)
    {
        heading.ResetText(contract.name);
        description.text = contract.description;

        string requirementsString = "";

        foreach(Requirement requirement in contract.requirements)
        {
            requirementsString += requirement.amount + "X " + requirement.item.name + "\n";
        }
        requirements.text = requirementsString;

        string rewardsString = "";

        if(contract.rewardGold != 0)
        {
            rewardsString += "Gold: " + contract.rewardGold + "\n";
        }

        foreach (HarvestDataTypes.Item rewardItem in contract.rewardItems)
        {
            rewardsString += rewardItem.name + "\n";
        }

        rewards.text = rewardsString;

        slotsController.SetNumberOfSlotsEnabled(contract.requirements.Count);
    }

    public void SetContract(Contract contract)
    {
        currentContract = contract;
        UpdateInformation(contract);
    }

    public Contract GetContract()
    {
        return currentContract;
    }

    public void OnEnable()
    {
        slotsController.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        slotsController.gameObject.SetActive(false);
    }
}
