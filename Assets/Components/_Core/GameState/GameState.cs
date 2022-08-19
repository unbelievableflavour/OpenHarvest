using HarvestDataTypes;
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
    public int buildNumber = 8; // Used for updating save files.
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
        { "resolutionScale", "0" },
        { "useShadows", "true" },
        { "useApplicationSpaceWarp", "true" }
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

    public Dictionary<string, List<Animal>> animals = new Dictionary<string, List<Animal>>
    {
        {"chickens", new List<Animal>()},
        {"cows", new List<Animal>()},
        {"sheep", new List<Animal>()},
        {"pigs", new List<Animal>()}
    };

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

    public float timeLastItemDropped = 0;// used for making sure stack doesn't increase twice.
    public string idLastItemDropped = "";// used for making sure stack doesn't increase twice.

    public string enteredSceneThrough = null;

    public Dictionary<string, int> unlockables = new Dictionary<string, int> { };
    public Dictionary<string, string> locationConfigurations = new Dictionary<string, string> {
        {"FireplaceDecoration", null},
        {"CornerDecoration1", null},
        {"CornerDecoration2", null},
        {"Poster1", null},
        {"Poster2", null},
        {"Poster3", null},
        {"GreenHouseSprinkler1", null},
        {"GreenHouseSprinkler2", null},
        {"GreenHouseSprinkler3", null},
        {"GreenHouseSprinkler4", null},
        {"GreenHouseSprinkler5", null},
        {"GreenHouseSprinkler6", null},
        {"TableDecoration1", null},
        {"ChairDecoration1", null},
        {"ChairDecoration2", null},
        {"ChairDecoration3", null},
        {"ChairDecoration4", null},
        {"BedDecoration1", null},
    };

    public Dictionary<Quests, Quest> questList = new Dictionary<Quests, Quest> { };

    public void InitializeUnlockablesAndQuests()
    {
        foreach (string unlockable in unlockables.Keys.ToList())
        {
            unlockables[unlockable] = 0;
        }

        LoadQuests();
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

        return unlockables[item.itemId] >= item.maximumTimesOwned;
    }
}
