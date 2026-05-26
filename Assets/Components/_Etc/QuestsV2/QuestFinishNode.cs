using UnityEngine;

[CreateNodeMenu("OpenHarvest/Quests/Finish Node")]
public class QuestFinishNode : QuestNodeBase
{
    [Header("Rewards")]
    [Min(0)]
    [Tooltip("Coins awarded when this quest node is completed.")]
    public int coinReward;

    [Tooltip("Optional item to spawn in the NPC's hand when quest is completed.")]
    public HarvestDataTypes.Item handRewardItem;

    private void OnValidate()
    {
        if (coinReward < 0)
        {
            coinReward = 0;
        }
    }

    public override void RunAction(InteractionUIController interactionUI, NpcProximityInteractable interactable, QuestGraph graph)
    {
        if (coinReward > 0 && GameState.Instance != null)
        {
            GameState.Instance.IncreaseMoneyByAmount(coinReward);
        }

        if (handRewardItem == null || interactable == null)
        {
            return;
        }

        NPCController npc = interactable.GetComponentInParent<NPCController>();
        if (npc == null)
        {
            npc = interactable.GetComponent<NPCController>();
        }

        if (npc == null)
        {
            return;
        }

        npc.SpawnQuestReward(handRewardItem);
    }
}
