using System.Collections.Generic;
using UnityEngine;

public static class Definitions
{
    public static int maxIntValue = 2147483647; // maximum value that an integer can possibly hold

    public enum TutorialStates
    {
        Initialized,
        GrabbedBackpack,
        GrabbedWallet,
        EnabledTooltip,
        Shoveled,
        Hoed,
        Seeded,
        Watered
    };

    public enum SoilStates
    {
        Initialized,
        InitializedWithObject,
        Shoveled,
        Plowed,
        Sowed,
    };

    public enum PlantStates
    {
        Seed,
        Sprout,
        YoungAdult,
        Adult,
        FullGrown
    };

    public enum CookingStates
    {
        notCookingHere,
        cookingShouldBeInitiated,
        cooking,
        done,
        burned,
    };

    public enum WindmillStates
    {
        Initialized,
        Grinding,
        Finished,
    };

    public enum Items { Axe }; // Definitions

    // Dont remove itesm from this list. Item locations will break if you do.
    public enum ItemLocations
    {
        FireplaceDecoration,
        CornerDecoration1,
        CornerDecoration2,
        Poster1,
        Poster2,
        Poster3,
        GreenHouseSprinkler1,
        GreenHouseSprinkler2,
        GreenHouseSprinkler3,
        GreenHouseSprinkler4,
        GreenHouseSprinkler5,
        GreenHouseSprinkler6,
        TableDecoration1,
        ChairDecoration1,
        ChairDecoration2,
        ChairDecoration3,
        ChairDecoration4,
        BedDecoration1,
        CornerDecoration,
    };

    public enum Quests
    {
        SantasLittleHelper,
        GettingBronzeForTheOldy,
        CleanupCrew,
        ShellFinder
    };

    public static bool ItemsAreLoaded = false;

    public static Dictionary<string, List<string>> itemsWithTypes = new Dictionary<string, List<string>> {
        {"fish", new List<string>()},
        {"plant", new List<string>()},
    };

    public static Dictionary<string, List<HarvestDataTypes.Item>> itemStores = new Dictionary<string, List<HarvestDataTypes.Item>> {
        {"storeAnimals", new List<HarvestDataTypes.Item>()},
        {"storeFishingTabFunctional", new List<HarvestDataTypes.Item>()},
        {"storeFishingTabDecorational", new List<HarvestDataTypes.Item>()},
        {"storeToolsTabFunctional", new List<HarvestDataTypes.Item>()},
        {"storeToolsTabDecorational", new List<HarvestDataTypes.Item>()},
        {"storeArtTabFunctional", new List<HarvestDataTypes.Item>()},
        {"storeArtTabDecorational", new List<HarvestDataTypes.Item>()},
        {"storeSeedsTabFunctional", new List<HarvestDataTypes.Item>()},
        {"storeSeedsTabDecorational", new List<HarvestDataTypes.Item>()},
        {"storeArtisanTabFunctional", new List<HarvestDataTypes.Item>()},
        {"storeArtisanTabDecorational", new List<HarvestDataTypes.Item>()},
        {"storeDecorationsTabFunctional", new List<HarvestDataTypes.Item>()},
        {"storeDecorationsTabDecorational", new List<HarvestDataTypes.Item>()},
        {"storeChristmasDecorationsTabDecorational", new List<HarvestDataTypes.Item>()},
        {"storeCookingTabDecorational", new List<HarvestDataTypes.Item>()},
        {"storeCookingTabFunctional", new List<HarvestDataTypes.Item>()}
    };

    static Definitions()
    {
        LoadQuests();
    }

    static public void LoadQuests()
    {

        Quest newQuest = new Quest()
        {
            id = Quests.SantasLittleHelper,
            title = "Santa's Little Helper",
            description = "Hang the christmas baubles in the tree!",
        };
        if (!GameState.Instance.questList.ContainsKey(newQuest.id))
        {
            GameState.Instance.questList.Add(newQuest.id, newQuest);
        }

        newQuest = new Quest()
        {
            id = Quests.GettingBronzeForTheOldy,
            title = "Getting Bronze For The Oldy",
            description = "Hang the christmas baubles in the tree!",
        };
        if (!GameState.Instance.questList.ContainsKey(newQuest.id))
        {
            GameState.Instance.questList.Add(newQuest.id, newQuest);
        }

        newQuest = new Quest()
        {
            id = Quests.CleanupCrew,
            title = "Cleanup Crew",
            description = "Clean that dirty beach.",
        };
        if (!GameState.Instance.questList.ContainsKey(newQuest.id))
        {
            GameState.Instance.questList.Add(newQuest.id, newQuest);
        }

        newQuest = new Quest()
        {
            id = Quests.ShellFinder,
            title = "Shell Finder",
            description = "Collect some awesome shells.",
        };
        if (!GameState.Instance.questList.ContainsKey(newQuest.id))
        {
            GameState.Instance.questList.Add(newQuest.id, newQuest);
        }
    }

    public static HarvestDataTypes.Item GetItemFromObject(GameObject itemObject){
        var itemInformation = itemObject.GetComponent<ItemInformation>();
        if (itemInformation == null) {
            return null;
        }

        return itemInformation.getItem();
    }

    public static HarvestDataTypes.Item GetItemFromObject(Transform itemObject){
        var itemInformation = itemObject.GetComponent<ItemInformation>();
        if (itemInformation == null) {
            return null;
        }

        return itemInformation.getItem();
    }

    public static HarvestDataTypes.Item GetItemFromObject(BNG.Grabbable itemObject){
        var itemInformation = itemObject.GetComponent<ItemInformation>();
        if (itemInformation == null) {
            return null;
        }

        return itemInformation.getItem();
    }

    public static GameObject InstantiateItemNew(GameObject prefab, Transform spawnLocation = null)
    {
        GameObject spawnedObject = Object.Instantiate(prefab) as GameObject;
        spawnedObject.name = prefab.name;

        if (spawnLocation)
        {
            spawnedObject.transform.position = spawnLocation.position;
        }
        return spawnedObject;
    }
}