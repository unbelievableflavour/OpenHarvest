using BNG;
using HarvestDataTypes;
using System.Collections.Generic;
using UnityEngine;

public class ContractSlotsController : MonoBehaviour
{
    public Transform[] slots;
    public ContractsDetailPage contractsDetailPage;
    public UnityEngine.UI.Button backButton;
    public UnityEngine.UI.Button completeButton;
    public AutoType errorText;
    public ParticleSystem finishEffect;
    public AutoType finishText;

    int currentFilledSlotsCount = 0;

    public void SetNumberOfSlotsEnabled(int numberOfSlots)
    {
        int i = 0;
        foreach(Transform slot in slots)
        {
            i++;
            slot.gameObject.SetActive(!(i > numberOfSlots));
        }
    }

    public void SnapItem()
    {
        currentFilledSlotsCount++;
        RefreshBackButton();
        completeButton.interactable = false;
        CancelInvoke("ValidateContract");
        Invoke("ValidateContract", 0.5f);
    }

    public void UnsnapItem()
    {
        currentFilledSlotsCount--;
        RefreshBackButton();
        completeButton.interactable = false;
        CancelInvoke("ValidateContract");
        Invoke("ValidateContract", 0.5f);
    }

    public void RefreshSlot()
    {
        completeButton.interactable = false;
        CancelInvoke("ValidateContract");
        Invoke("ValidateContract", 0.5f);
    }

    private void RefreshBackButton()
    {
        if (currentFilledSlotsCount > 0)
        {
            backButton.interactable = false;
        }
        else
        {
            backButton.interactable = true;
        }
    }

    void ValidateContract()
    {
        List<Requirement> requirementsInSlots = new List<Requirement>();

        foreach (Transform slot in slots)
        {
            SnapZone snapZone = slot.GetComponent<SnapZone>();
            var heldItem = snapZone.HeldItem;
            if (!heldItem)
            {
                continue;
            }

            Requirement requirementInSlot = new Requirement();
            requirementInSlot.item = Definitions.GetItemFromObject(snapZone.HeldItem);

            requirementInSlot.amount = 1;
            if (heldItem.GetComponent<ItemStack>() && heldItem.GetComponent<ItemStack>().GetStackSize() > 1)
            {
                requirementInSlot.amount = heldItem.GetComponent<ItemStack>().GetStackSize();
            }

            requirementsInSlots.Add(requirementInSlot);
        }

        if (requirementsInSlots.Count == 0)
        {
            errorText.ResetText("Add items to the slots");
            errorText.Refresh();
            return;
        }

        foreach (Requirement requirementInContract in contractsDetailPage.GetContract().requirements)
        {
            bool requirementMet = false;
            foreach (Requirement requirementInSlot in requirementsInSlots)
            {
                if(requirementInContract.amount == requirementInSlot.amount 
                    && requirementInContract.item == requirementInSlot.item)
                {
                    requirementMet = true;
                    break;
                }
            }

            if (!requirementMet)
            {
                errorText.ResetText("Requirements are not met");
                errorText.Refresh();
                return;
            }
        }

        errorText.ResetText("Valid! Click complete to spawn reward!");
        errorText.Refresh();
        completeButton.interactable = true;
    }

    public void SpawnReward()
    {
        errorText.ResetText("Add items to the slots");
        errorText.Refresh();
        completeButton.interactable = false;
        backButton.interactable = true;
        finishEffect.Play();   

        if (contractsDetailPage.GetContract().rewardGold > 1)
        {
            GameState.Instance.IncreaseMoneyByAmount(contractsDetailPage.GetContract().rewardGold);
            AudioManager.Instance.PlayClip("sell");
            finishText.ResetText("You've gained " + contractsDetailPage.GetContract().rewardGold + " gold");
            finishText.gameObject.SetActive(true);
            Invoke("DisableMoneyGainedText", 2f);
        }

        RemoveAllItemsFromSlots();
        RemoveContractFromWeeklyContracts();
    }

    private void RemoveAllItemsFromSlots()
    {
        foreach (Transform slot in slots)
        {
            SnapZone snapZone = slot.GetComponent<SnapZone>();
            var heldItem = snapZone.HeldItem;
            if (heldItem)
            {
                heldItem.DropItem(false, false);
                Destroy(heldItem.gameObject);
            }
        }
    }

    private void RemoveContractFromWeeklyContracts()
    {
        var currentContract = contractsDetailPage.GetContract();
        GameState.Instance.contractsOfTheWeek.currentContracts.Remove(currentContract);
    }

    void DisableMoneyGainedText()
    {
        finishText.gameObject.SetActive(false);
    }
}
