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

    public static Dictionary<string, List<HarvestDataTypes.StoreProduct>> itemStores = new Dictionary<string, List<HarvestDataTypes.StoreProduct>> {
        {"storeAnimals", new List<HarvestDataTypes.StoreProduct>()},
        {"storeFishingTabFunctional", new List<HarvestDataTypes.StoreProduct>()},
        {"storeFishingTabDecorational", new List<HarvestDataTypes.StoreProduct>()},
        {"storeToolsTabFunctional", new List<HarvestDataTypes.StoreProduct>()},
        {"storeToolsTabDecorational", new List<HarvestDataTypes.StoreProduct>()},
        {"storeArtTabFunctional", new List<HarvestDataTypes.StoreProduct>()},
        {"storeArtTabDecorational", new List<HarvestDataTypes.StoreProduct>()},
        {"storeSeedsTabFunctional", new List<HarvestDataTypes.StoreProduct>()},
        {"storeSeedsTabDecorational", new List<HarvestDataTypes.StoreProduct>()},
        {"storeArtisanTabFunctional", new List<HarvestDataTypes.StoreProduct>()},
        {"storeArtisanTabDecorational", new List<HarvestDataTypes.StoreProduct>()},
        {"storeDecorationsTabFunctional", new List<HarvestDataTypes.StoreProduct>()},
        {"storeDecorationsTabDecorational", new List<HarvestDataTypes.StoreProduct>()},
        {"storeChristmasDecorationsTabDecorational", new List<HarvestDataTypes.StoreProduct>()},
        {"storeCookingTabDecorational", new List<HarvestDataTypes.StoreProduct>()},
        {"storeCookingTabFunctional", new List<HarvestDataTypes.StoreProduct>()}
    };

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