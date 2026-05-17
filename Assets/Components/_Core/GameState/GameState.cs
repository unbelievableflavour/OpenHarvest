using HarvestDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Definitions;

public class GameState: MonoBehaviour
{
	public static CurrentGameState Instance = null;

	private void Awake()
	{
		if (Instance != null)
		{
            return;
        }

        Reset();
	}

    public static void Reset()
    {
        Instance = new CurrentGameState();
        Instance.InitializeUnlockablesAndQuests();
    }
}

public class CurrentGameState {
    public int buildNumber = 9; // Used for updating save files.
    public int saveNumber;

    public Dictionary<string, string> settings = new Dictionary<string, string> {
        { "useHandsOnly", "false" },
        { "useSmoothLocomotion", "false" },
        { "useSmoothTurning", "false" },
        { "smoothTurningSensitivity", "10" },
        { "useAssistMode", "true" },
        { "playerHeightOffset", "0" },
        { "backgroundMusicVolume", "1" },
        { "skinColor", "#FDD7C3" },
        { "refreshRate", "2" }, // 2 = refresh rate 90
        { "resolutionScale", "2" }, // 2 = resolution scale 1.0f
        { "useShadows", "true" },
        { "useFog", "true" },
        { "useApplicationSpaceWarp", "false" } // Removed - was Oculus-specific
    };

    public int currentDay;

    public Dictionary<string, RespawningObject> respawningObjects = new Dictionary<string, RespawningObject>();

    public Dictionary<string, List<SaveablePatch>> soilGrids = new Dictionary<string, List<SaveablePatch>> {
        {"farm", new List<SaveablePatch>()},
        {"greenhouse1", new List<SaveablePatch>()},
        {"greenhouse2", new List<SaveablePatch>()},
        {"greenhouse3", new List<SaveablePatch>()},
        {"greenhouse4", new List<SaveablePatch>()},
        {"greenhouse5", new List<SaveablePatch>()},
        {"greenhouse6", new List<SaveablePatch>()}
    };

    public Dictionary<string, List<SaveableItem>> itemStashes = new Dictionary<string, List<SaveableItem>>
    {
        {"backpack", new List<SaveableItem>()},
        {"basket", new List<SaveableItem>()},
        {"storageCrate", new List<SaveableItem>()},
        {"christmasTree", new List<SaveableItem>()},
        {"hat", new List<SaveableItem>()}
    };

    public Dictionary<string, List<SaveablePlacedObject>> placedObjectsByScene = new Dictionary<string, List<SaveablePlacedObject>>();

    public List<Animal> animals = new List<Animal>();

    public SaveableWindmill windmill;

    public string name;
    public string farmName;

    public Dictionary<string, ItemOfTheWeek> itemsOfTheWeek = new Dictionary<string, ItemOfTheWeek>
    {
        {"plant", new ItemOfTheWeek() { currentItemId = "Tomato", nextItemId = "Carrot" } },
        {"fish", new ItemOfTheWeek() { currentItemId = "Salmon", nextItemId = "Bass" } }
    };

    public ContractsOfTheWeek contractsOfTheWeek = new ContractsOfTheWeek() 
    { 
        currentContracts = new List<Contract>(), nextContracts = new List<Contract>() 
    };

    private int totalAmount = 0;

    public Transform currentPlayerPosition;

    public string followingNpcId = null; // NPC id to get from the DB and spawn in new scenes when following.
    public string followingNpcInstanceId = null; // UniqueId of the specific scene instance; set for animals where multiple exist per scene

    public float timeLastItemDropped = 0;// used for making sure stack doesn't increase twice.
    public string idLastItemDropped = "";// used for making sure stack doesn't increase twice.

    public string enteredSceneThrough = null;

    public Dictionary<string, int> unlockables = new Dictionary<string, int> { };

    public Dictionary<string, QuestRuntimeProgressState> questRuntimeStates = new Dictionary<string, QuestRuntimeProgressState>();

    public void InitializeUnlockablesAndQuests()
    {
        // reset all quests and unlockables for when switching games
        unlockables = new Dictionary<string, int> { };
    }

    public void IncreaseMoneyByAmount(int amount)
    {
        if ((totalAmount + amount) > maxIntValue)
        {
            totalAmount = maxIntValue;
            return;
        }

        if ((totalAmount + amount) < 0)
        {
            totalAmount = maxIntValue;
            return;
        }

        totalAmount += amount;
    }

    public void DecreaseMoneyByAmount(int amount)
    {
        totalAmount -= amount;
    }

    public int getTotalAmount()
    {
        return totalAmount;
    }

    public bool isUnlocked(string itemId)
    {
        return unlockables.ContainsKey(itemId) && unlockables[itemId] > 0;
    }

    public void unlock(string itemId, int amount)
    {
        if (!unlockables.ContainsKey(itemId)) {
            unlockables.Add(itemId, amount);
            return;
        }
        unlockables[itemId] += amount;
    }

    public bool ownsMaximumNumber(HarvestDataTypes.Item item)
    {
        if (item.maximumTimesOwned == null)
        {
            return false;
        }

        if (!unlockables.ContainsKey(item.itemId)) {
            return false;
        }

        return unlockables[item.itemId] >= item.maximumTimesOwned;
    }

    public bool ownsMaximumNumber(string unlockableId, int maximumTimesOwned)
    {
        if (maximumTimesOwned <= 0 || string.IsNullOrWhiteSpace(unlockableId))
        {
            return false;
        }

        if (!unlockables.ContainsKey(unlockableId))
        {
            return false;
        }

        return unlockables[unlockableId] >= maximumTimesOwned;
    }

    public event Action OnToggleMode;
    private string currentMode = "default";

    public string GetMode() {
        return currentMode;
    }
    
    public void SwitchToMode(string newMode) {
        currentMode = newMode;
        GameState.Instance.OnToggleMode?.Invoke();
    }
}

[Serializable]
public class QuestRuntimeProgressState
{
    public int currentNodeIndex = -1;
    public int pendingGiftNodeIndex = -1;
    public bool isCompleted;
}
