using System.Collections.Generic;
using UnityEngine;

public static class Definitions
{
    public static string ResourcesFolder = "Items/";
    public static string fallbackItemId = "BuyItemFallback";

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
        CornerDecoration, //Old enum. Readded for backwards compatibility. Stijn you cannot just remove enums. Discuss that shit with me.
    };

    public enum Quests
    {
        SantasLittleHelper,
        GettingBronzeForTheOldy,
        CleanupCrew,
        ShellFinder
    };

    public static Dictionary<string, Item> ItemsWithInformation = new Dictionary<string, Item>();

    public static Dictionary<string, List<string>> itemsWithTypes = new Dictionary<string, List<string>> {
        {"fish", new List<string>()},
        {"plant", new List<string>()},
    };

    public static Dictionary<string, List<string>> itemStores = new Dictionary<string, List<string>> {
        {"vendingMachine", new List<string>()},
        {"storeAnimals", new List<string>()},
        {"storeFishingTabFunctional", new List<string>()},
        {"storeFishingTabDecorational", new List<string>()},
        {"storeToolsTabFunctional", new List<string>()},
        {"storeToolsTabDecorational", new List<string>()},
        {"storeArtTabFunctional", new List<string>()},
        {"storeArtTabDecorational", new List<string>()},
        {"storeSeedsTabFunctional", new List<string>()},
        {"storeSeedsTabDecorational", new List<string>()},
        {"storeArtisanTabFunctional", new List<string>()},
        {"storeArtisanTabDecorational", new List<string>()},
        {"storeDecorationsTabFunctional", new List<string>()},
        {"storeDecorationsTabDecorational", new List<string>()},
        {"storeChristmasDecorationsTabDecorational", new List<string>()},
        {"storeCookingTabDecorational", new List<string>()},
        {"storeCookingTabFunctional", new List<string>()}
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
        if (!GameState.questList.ContainsKey(newQuest.id))
        {
            GameState.questList.Add(newQuest.id, newQuest);
        }

        newQuest = new Quest()
        {
            id = Quests.GettingBronzeForTheOldy,
            title = "Getting Bronze For The Oldy",
            description = "Hang the christmas baubles in the tree!",
        };
        if (!GameState.questList.ContainsKey(newQuest.id))
        {
            GameState.questList.Add(newQuest.id, newQuest);
        }

        newQuest = new Quest()
        {
            id = Quests.CleanupCrew,
            title = "Cleanup Crew",
            description = "Clean that dirty beach.",
        };
        if (!GameState.questList.ContainsKey(newQuest.id))
        {
            GameState.questList.Add(newQuest.id, newQuest);
        }

        newQuest = new Quest()
        {
            id = Quests.ShellFinder,
            title = "Shell Finder",
            description = "Collect some awesome shells.",
        };
        if (!GameState.questList.ContainsKey(newQuest.id))
        {
            GameState.questList.Add(newQuest.id, newQuest);
        }   
    }

    public static Item GetItemInformation(string itemId)
    {
        try
        {
            return ItemsWithInformation[itemId];
        } catch (System.Exception e) {
            throw new System.Exception("Item with id: " + itemId + " was not found in item dictionary.");
        }
    }

    public static GameObject InstantiateItem(string resourceName, Transform spawnLocation = null)
    {
        var objectToInstantiate = Resources.Load(ResourcesFolder + resourceName + "/" + resourceName, typeof(GameObject));

        if(objectToInstantiate == null)
        {
            throw new System.Exception("Error trying to instantiate prefab: " + resourceName);
        }

        GameObject spawnedObject = Object.Instantiate(objectToInstantiate) as GameObject;
        spawnedObject.name = resourceName;

        if (spawnLocation)
        {
            spawnedObject.transform.position = spawnLocation.position;
        }
        return spawnedObject;
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