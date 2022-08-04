using HarvestDataTypes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Definitions;

public static class GameState
{
    //Remember! Values should be set in the InitializeValues function. NOT in the variables below.

    public static int buildNumber = 8; //Used for updating save files.
    public static int saveNumber;

    public static Dictionary<string, string> settings;

    public static int currentDay;

    public static Dictionary<string, RespawningObject> respawningObjects;

    public static Dictionary<string, List<SaveablePatch>> soilGrids;
    public static Dictionary<string, List<SaveableItem>> itemStashes;
    public static Dictionary<string, List<Animal>> animals;

    public static SaveableWindmill windmill;

    public static string name;
    public static string farmName;

    public static Dictionary<string, ItemOfTheWeek> itemsOfTheWeek;
    public static ContractsOfTheWeek contractsOfTheWeek;

    private static int totalAmount;

    public static SceneSwitcher currentSceneSwitcher;
    public static Transform currentPlayerPosition;

    public static float timeLastItemDropped;// used for making sure stack doesn't increase twice.
    public static string idLastItemDropped;// used for making sure stack doesn't increase twice.

    public static string enteredSceneThrough;

    public static Dictionary<string, int> unlockables;
    public static Dictionary<string, string> locationConfigurations;

    public static Dictionary<Quests, Quest> questList;

    static GameState()
    {
        InitializeValues();
    }

    private static void InitializeValues()
    {
        totalAmount = 0;
        respawningObjects = new Dictionary<string, RespawningObject>();

        settings = new Dictionary<string, string> {
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

        soilGrids = new Dictionary<string, List<SaveablePatch>> {
            {"farm", new List<SaveablePatch>()},
            {"greenhouse1", new List<SaveablePatch>()},
            {"greenhouse2", new List<SaveablePatch>()},
            {"greenhouse3", new List<SaveablePatch>()},
            {"greenhouse4", new List<SaveablePatch>()},
            {"greenhouse5", new List<SaveablePatch>()},
            {"greenhouse6", new List<SaveablePatch>()}
        };

        itemStashes = new Dictionary<string, List<SaveableItem>>
        {
            {"backpack", new List<SaveableItem>()},
            {"basket", new List<SaveableItem>()},
            {"storageCrate", new List<SaveableItem>()},
            {"christmasTree", new List<SaveableItem>()},
            {"hat", new List<SaveableItem>()}
        };

        animals = new Dictionary<string, List<Animal>>
        {
            {"chickens", new List<Animal>()},
            {"cows", new List<Animal>()},
            {"sheep", new List<Animal>()},
            {"pigs", new List<Animal>()}
        };

        itemsOfTheWeek = new Dictionary<string, ItemOfTheWeek>
        {
            {"plant", new ItemOfTheWeek() { currentItemId = "Tomato", nextItemId = "Carrot" } },
            {"fish", new ItemOfTheWeek() { currentItemId = "Salmon", nextItemId = "Bass" } }
        };

        contractsOfTheWeek = new ContractsOfTheWeek() { currentContracts = new List<Contract>(), nextContracts = new List<Contract>() };

        timeLastItemDropped = 0;// used for making sure stack doesn't increase twice.
        idLastItemDropped = "";// used for making sure stack doesn't increase twice.

        locationConfigurations = new Dictionary<string, string> {
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

        if(unlockables == null)
        {
            unlockables = new Dictionary<string, int> { };
        } else {
            foreach (string unlockable in unlockables.Keys.ToList())
            {
                unlockables[unlockable] = 0;
            }
        }

        questList = new Dictionary<Quests, Quest> { };
        LoadQuests();
    }

    public static void IncreaseMoneyByAmount(int amount)
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

    public static void DecreaseMoneyByAmount(int amount)
    {
        totalAmount -= amount;
    }

    public static int getTotalAmount()
    {
        return totalAmount;
    }

    public static bool isUnlockable(string itemId)
    {
        return DatabaseManager.Instance.items.FindById(itemId).isUnlockable;
    }

    public static bool isUnlocked(string itemId)
    {
        return unlockables.ContainsKey(itemId) && unlockables[itemId] > 0;
    }

    public static void unlock(string itemId, int amount)
    {
        if (!unlockables.ContainsKey(itemId)) {
            unlockables.Add(itemId, amount);
            return;
        }
        unlockables[itemId] += amount;
    }

    public static bool ownsMaximumNumber(Item item)
    {
        if (item.maximumTimesOwned == null)
        {
            return false;
        }

        return unlockables[item.itemId] >= item.maximumTimesOwned;
    }

    public static void Reset()
    {
        InitializeValues();
    }
}
