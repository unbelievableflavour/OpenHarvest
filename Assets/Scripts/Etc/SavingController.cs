using HarvestDataTypes;
using OVRSimpleJSON;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using static Definitions;

public class SavingController : MonoBehaviour
{
    private static SaveGame CreateSaveGameObject()
    {
        SaveGame save = new SaveGame();

        save.buildNumber = GameState.Instance.buildNumber;
        save.saveNumber = GameState.Instance.saveNumber;

        save.settings = GameState.Instance.settings;

        save.currentDay = GameState.Instance.currentDay;
        save.name = GameState.Instance.name;
        save.farmName = GameState.Instance.farmName;
        save.windmill = GameState.Instance.windmill;

        save.respawningObjects = GameState.Instance.respawningObjects;
        save.soilGrids = GameState.Instance.soilGrids;
        save.itemStashes = GameState.Instance.itemStashes;
        save.animals = GameState.Instance.animals;
        save.itemsOfTheWeek = GameState.Instance.itemsOfTheWeek;
        save.contractsOfTheWeek = GameState.Instance.contractsOfTheWeek.ToSaveable();
        save.questList = GameState.Instance.questList;

        save.money = GameState.Instance.getTotalAmount();

        save.unlockables2 = GameState.Instance.unlockables;
        save.locationConfigurations2 = GameState.Instance.locationConfigurations;

        save.totalSecondsSpentIngame = TimeController.GetTotalSecondsSpentIngame();
        save.totalSimulatedGameSeconds = TimeController.GetTotalSimulatedGameSeconds();
        save.useRealTime = TimeController.isPlayingInRealTime();

        save.weatherToday = WeatherController.weatherToday;
        save.weatherTomorrow = WeatherController.weatherTomorrow;

        return save;
    }

    public static void SaveGame()
    {
        SaveGame save = CreateSaveGameObject();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(getSavePath() + GameState.Instance.saveNumber + ".save");
        bf.Serialize(file, save);
        file.Close();
    }

    public static void LoadGame(int saveNumber)
    {
        GameState.Reset();
        if (!SaveExists(saveNumber))
        {
            Debug.Log("No game saved!");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(getSavePath() + saveNumber + ".save", FileMode.Open);
        SaveGame save = (SaveGame)bf.Deserialize(file);
        file.Close();

        GameState.Instance.saveNumber = save.saveNumber;

        GameState.Instance.IncreaseMoneyByAmount(save.money);
        GameState.Instance.windmill = save.windmill;

        GameState.Instance.respawningObjects = save.respawningObjects;
        GameState.Instance.soilGrids = save.soilGrids;
        GameState.Instance.itemStashes = save.itemStashes;
        GameState.Instance.animals = save.animals;

        GameState.Instance.itemsOfTheWeek = save.itemsOfTheWeek;

        GameState.Instance.contractsOfTheWeek = new ContractsOfTheWeek();
        GameState.Instance.contractsOfTheWeek.ImportSaveable(save.contractsOfTheWeek);
        
        GameState.Instance.unlockables = save.unlockables2;

        //Following is set in reset. So it shouldnt be necessary here.
//        GameState.Instance.questList = new Dictionary<Quests, Quest> { };
//        LoadQuests();

        foreach (var quest in save.questList)
            GameState.Instance.questList[quest.Key] = quest.Value;

        foreach (var locationConfig in save.locationConfigurations2)
            GameState.Instance.locationConfigurations[locationConfig.Key] = locationConfig.Value;

        GameState.Instance.name = save.name;
        GameState.Instance.farmName = save.farmName;

        TimeController.SetTotalSimulatedGameSeconds(save.totalSimulatedGameSeconds);
        TimeController.SetTotalSecondsSpentIngame(save.totalSecondsSpentIngame);

        if (save.useRealTime) {
            TimeController.SetPlayInRealTime(); // keep both here so that static vars wont mix
        } else {
            TimeController.SetPlayInSimulatedTime(); // keep both here so that static vars wont mix
        }

        WeatherController.weatherToday = save.weatherToday;
        WeatherController.weatherTomorrow = save.weatherTomorrow;

        foreach (var setting in save.settings)
            GameState.Instance.settings[setting.Key] = setting.Value;

        PlayerCustomSettings.Instance.RefreshSettings();
    }
    public static void DeleteGame(int saveNumber)
    {
        File.Delete(getSavePath() + saveNumber + ".save");
    }

    public static SaveGame GetSaveInformation(int saveNumber)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(getSavePath() + saveNumber + ".save", FileMode.Open);
        SaveGame rawSave = (SaveGame)bf.Deserialize(file);
        file.Close();

        SaveGame updatedSave = UpdateGameSaveToLatestVersion(rawSave);

        return updatedSave;
    }

    private static SaveGame UpdateGameSaveToLatestVersion(SaveGame save)
    {
        Debug.Log("Run migrations if needed for save: " + save.saveNumber);
        
        //Migration in build unknown.. (probably old)
        if (save.respawningObjects == null)
        {
            save.respawningObjects = GameState.Instance.respawningObjects;
        }

        if (save.itemStashes == null)
        {
            save.itemStashes = GameState.Instance.itemStashes;
        }

        if (save.soilGrids == null)
        {
            save.soilGrids = GameState.Instance.soilGrids;
        }

        if (save.questList == null)
        {
            save.questList = GameState.Instance.questList;
        }

        if (save.name == null)
        {
            save.name = "Bob";
        }

        if (save.farmName == null)
        {
            save.farmName = "Awesome";
        }

        if (save.settings == null)
        {
            save.settings = new Dictionary<string, string>() {};
        }

        if (save.weatherToday == null)
        {
            save.weatherToday = WeatherController.SUNNY;
        }

        if (save.weatherTomorrow == null)
        {
            save.weatherTomorrow = WeatherController.RAINY;
        }

        //Migration to build 2
        if (save.buildNumber < 2)
        {
            Debug.Log("Run migration for build 2 on save: " + save.saveNumber);

            var mapper = new TemporaryPrefabToIdMapper();
            foreach (SaveableItem item in save.itemsStoredInChristmasTree)
            {
                save.itemStashes["christmasTree"].Add(mapper.GetUpdatedItem(item));
            }
            foreach (SaveableItem item in save.itemsStoredInBasket)
            {
                save.itemStashes["basket"].Add(mapper.GetUpdatedItem(item));
            }
            foreach (SaveableItem item in save.itemsStoredInBackpack)
            {
                save.itemStashes["backpack"].Add(mapper.GetUpdatedItem(item));
            }
            foreach (SaveableItem item in save.itemsStoredInStorageCrate)
            {
                save.itemStashes["storageCrate"].Add(mapper.GetUpdatedItem(item));
            }

            save.soilGrids["farm"] = save.patches;
        }

        //Migration to build 3
        if (save.buildNumber < 3)
        {    
            Debug.Log("Run migration for build 3 on save: " + save.saveNumber);

            save.itemStashes["hat"] = new List<SaveableItem>();
            save.currentHat = null;

            save.animals = new Dictionary<string, List<Animal>>
            {
                {"chickens", save.chickens == null ? new List<Animal>() : save.chickens},
                {"cows", save.cows == null ? new List<Animal>() : save.cows},
                {"sheep", save.sheep == null ? new List<Animal>() : save.sheep},
                {"pigs", save.pigs == null ? new List<Animal>() : save.pigs},
            };
            save.pigs = null;
            save.chickens = null;
            save.sheep = null;
            save.pigs = null;
        }

        //Migration to build 4
        if (save.buildNumber < 4)
        {
            Debug.Log("Run migration for build 4 on save: " + save.saveNumber);

            save.itemsOfTheWeek = new Dictionary<string, ItemOfTheWeek>
            {
                {"plant", new ItemOfTheWeek() { currentItemId = "Tomato", nextItemId = "Carrot" } },
                {"fish", new ItemOfTheWeek() { currentItemId = "Salmon", nextItemId = "Bass" } }
            };
            save.fishOfTheWeek = null;
            save.plantOfTheWeek = null;
        }

        //Migration to build 5
        if (save.buildNumber < 5)
        {
            Debug.Log("Run migration for build 5 on save: " + save.saveNumber);

            if(save.animals == null)
            {
                //fallback if a previous save failed.
                save.animals = new Dictionary<string, List<Animal>>
                {
                    {"chickens", save.chickens == null ? new List<Animal>() : save.chickens},
                    {"cows", save.cows == null ? new List<Animal>() : save.cows},
                    {"sheep", save.sheep == null ? new List<Animal>() : save.sheep},
                    {"pigs", save.pigs == null ? new List<Animal>() : save.pigs},
                };
            }

            foreach (Animal animal in save.animals["chickens"])
            {
                if (animal == null)
                {
                    continue;
                }
                animal.id = Items.Axe;
            }

            foreach (Animal animal in save.animals["cows"])
            {
                if (animal == null)
                {
                    continue;
                }
                animal.id = Items.Axe;
            }

            foreach (Animal animal in save.animals["sheep"])
            {
                if (animal == null)
                {
                    continue;
                }
                animal.id = Items.Axe;
            }

            foreach (Animal animal in save.animals["pigs"])
            {
                if(animal == null)
                {
                    continue;
                }
                animal.id = Items.Axe;
            }

            save.unlockables2 = new Dictionary<string, int> { };

            if (save.unlockables != null)
            {

                JSONNode jsonObject = JSON.Parse(save.unlockables);
                if (jsonObject != null && jsonObject["unlockables"] != null)
                {
                    foreach (var item in jsonObject["unlockables"])
                    {
                        save.unlockables2[item.Key] = item.Value;
                    }
                }
            }

            save.unlockables = null;   
        }

        //Migration to build 6
        if (save.buildNumber < 6)
        {
            Debug.Log("Run migration for build 6 on save: " + save.saveNumber);

            save.locationConfigurations2 = new Dictionary<string, string> { };

            if (save.locationConfigurations != null)
            {
                JSONNode jsonObject = JSON.Parse(save.locationConfigurations);
                if (jsonObject != null && jsonObject["locationConfigurations"] != null)
                {
                    foreach (var item in jsonObject["locationConfigurations"])
                    {
                        if (item.Value == "")
                        {
                            continue;
                        }

                        save.locationConfigurations2[item.Key] = item.Value;
                    }
                }
            }

            save.locationConfigurations = null;
        }

        //Migration to build 7
        if (save.buildNumber < 7)
        {
            Debug.Log("Run migration for build 7 on save: " + save.saveNumber);

            foreach (SaveablePatch item in save.soilGrids["farm"])
            {
                item.growthState2 = (int)item.growthState;
                item.growthState = 0;
            }
            foreach (SaveablePatch item in save.soilGrids["greenhouse1"])
            {
                item.growthState2 = (int)item.growthState;
                item.growthState = 0;
            }
            foreach (SaveablePatch item in save.soilGrids["greenhouse2"])
            {
                item.growthState2 = (int)item.growthState;
                item.growthState = 0;
            }
            foreach (SaveablePatch item in save.soilGrids["greenhouse3"])
            {
                item.growthState2 = (int)item.growthState;
                item.growthState = 0;
            }
            foreach (SaveablePatch item in save.soilGrids["greenhouse4"])
            {
                item.growthState2 = (int)item.growthState;
                item.growthState = 0;
            }
            foreach (SaveablePatch item in save.soilGrids["greenhouse5"])
            {
                item.growthState2 = (int)item.growthState;
                item.growthState = 0;
            }
            foreach (SaveablePatch item in save.soilGrids["greenhouse6"])
            {
                item.growthState2 = (int)item.growthState;
                item.growthState = 0;
            }
        }

        //Migration to build 8
        if (save.buildNumber < 8)
        {
            Debug.Log("Run migration for build 8 on save: " + save.saveNumber);

            save.contractsOfTheWeek = new SaveableContractsOfTheWeek() {
                currentContracts = new List<SaveableContract>(), 
                nextContracts = new List<SaveableContract>() 
            };
        }

        save.buildNumber = GameState.Instance.buildNumber;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(getSavePath() + save.saveNumber + ".save");
        bf.Serialize(file, save);
        file.Close();

        return save;
    }

    public static bool SaveExists(int saveNumber)
    {
        return File.Exists(getSavePath() + saveNumber + ".save");
    }

    private static string getSavePath()
    {
        return Application.persistentDataPath + "/gamesave";
    }
}

public class TemporaryPrefabToIdMapper
{
    Dictionary<string, string> oldPrefabToIdMapper = new Dictionary<string, string>() {
        { "Axe", "Axe" },
        { "Hammer", "Hammer" },
        { "Sickle", "Sickle" },
        { "SeedCabbage", "SeedBagCabbage" },
        { "SeedCorn", "SeedBagCorn" },
        { "SeedTurnip", "SeedBagTurnip" },
        { "SeedCucumber", "SeedBagCucumber" },
        { "SeedEggplant", "SeedBagEggplant" },
        { "Carrot", "Carrot" },
        { "Turnip", "Turnip" },
        { "Tomato", "Tomato" },
        { "Eggplant", "Eggplant" },
        { "Cucumber", "Cucumber" },
        { "Corn", "Corn" },
        { "Cabbage", "Cabbage" },
        { "FishSalmon", "Salmon" },
        { "Egg", "Egg" },
        { "Strawberry", "Strawberry" },
        { "SeedStrawberry", "SeedBagStrawberry" },
        { "FishBass", "Bass" },
        { "FishPike", "Pike" },
        { "Apple", "Apple" },
        { "GoldenEgg", "GoldenEgg" },
        { "SeedTomato", "SeedBagTomato" },
        { "SeedCarrot", "SeedBagCarrot" },
        { "Hoe", "Hoe" },
        { "Shovel", "Shovel" },
        { "WateringCan", "WateringCan" },
        { "FishingRod", "FishingRod" },
        { "WoodenBow", "WoodenBow" },
        { "HandShovel", "HandShovel" },
        { "AnimalFoodChicken", "ChickenFood" },
        { "Bottle", "Bottle" },
        { "Can", "Can" },
        { "GuideBook", "GuideBook" },
        { "Backpack", "Backpack" },
        { "BackpackBig", "BackpackBig" },
        { "Basket", "Basket" },
        { "HatStraw", "HatStraw" },
        { "HatFarmer", "HatFarmer" },
        { "HatBaret", "HatBaret" },
        { "HatCrown", "HatCrown" },
        { "HatChicken", "HatChicken" },
        { "HatCloud", "HatCloud" },
        { "HatFish", "HatFish" },
        { "HatFishing", "HatFishing" },
        { "HatFlatCap", "HatFlatCap" },
        { "HatFlying", "HatFlying" },
        { "HatHelmet", "HatHelmet" },
        { "HatSombrero", "HatSombrero" },
        { "HatTop", "HatTop" },
        { "HatViking", "HatViking" },
        { "HatWinter", "HatWinter" },
        { "Wallet", "Wallet" },
        { "HatPlain", "HatPlain" },
        { "Cauliflower", "Cauliflower" },
        { "Broccoli", "Broccoli" },
        { "Pumpkin", "Pumpkin" },
        { "Radish", "Radish" },
        { "SeedCauliflower", "SeedBagCauliflower" },
        { "SeedRadish", "SeedBagRadish" },
        { "SeedPumpkin", "SeedBagPumpkin" },
        { "SeedBroccoli", "SeedBagBroccoli" },
        { "FryingPan", "FryingPan" },
        { "FishSalmonBurned", "SalmonBurned" },
        { "FishSalmonFried", "SalmonFried" },
        { "FishBassBurned", "BassBurned" },
        { "FishSpike", "SpikeFish" },
        { "FishSpikeFried", "SpikeFishFried" },
        { "FishSpikeBurned", "SpikeFishBurned" },
        { "FishMoon", "MoonFish" },
        { "FishMoonFried", "MoonFishFried" },
        { "FishMoonBurned", "MoonFishBurned" },
        { "FishEel", "EelFish" },
        { "FishEelFried", "EelFishFried" },
        { "FishEelBurned", "EelFishBurned" },
        { "FishBassFried", "BassFried" },
        { "FishPikeBurned", "PikeBurned" },
        { "FishPikeFried", "PikeFried" },
        { "CornFried", "CornFried" },
        { "CornBurned", "CornBurned" },
        { "Sunflower", "Sunflower" },
        { "Watermelon", "Watermelon" },
        { "Banana", "Banana" },
        { "Pineaple", "Pineapple" },
        { "SeedSunflower", "SeedBagSunflower" },
        { "SeedWatermelon", "SeedBagWatermelon" },
        { "SeedBanana", "SeedBagBanana" },
        { "SeedPineapple", "SeedBagPineapple" },
        { "Coconut", "Coconut" },
        { "Bucket", "Bucket" },
        { "BeachBall", "BeachBall" },
        { "Pickaxe", "Pickaxe" },
        { "PickaxeBronze", "Pickaxe" },
        { "OreBronze", "OreBronze" },
        { "OreSilver", "OreSilver" },
        { "OreGold", "OreGold" },
        { "PumpkinRocket", "PumpkinRocket" },
        { "Lantern", "Lantern" },
        { "Torch", "Torch" },
        { "SeedHyacinth", "SeedBagHyacinth" },
        { "SeedRose", "SeedBagRose" },
        { "SeedTulip", "SeedBagTulip" },
        { "Hyacinth", "Hyacinth" },
        { "Rose", "Rose" },
        { "Tulip", "Tulip" },
        { "MushroomRed", "ShroomRed" },
        { "MushroomBlue", "ShroomBlue" },
        { "MushroomGreen", "ShroomGreen" },
        { "MushroomGreenShort", "ShroomGreenShort" },
        { "ChristmasBauble", "ChristmasBauble" },
        { "Wheat", "Wheat" },
        { "SeedWheat", "SeedBagWheat" },
        { "HatSanta", "HatSanta" },
        { "HatMiner", "HatMiner" },
        { "ChristmasBauble2", "ChristmasBauble2" },
        { "ChristmasBauble3", "ChristmasBauble3" },
        { "ChristmasPeak", "ChristmasPeak" },
        { "FishCandyCane", "CandyCaneFish" },
        { "FishCandyCaneFried", "CandyCaneFishFried" },
        { "FishCandyCaneBurned", "CandyCaneFishBurned" },
        { "BuyItemFallback", "BuyItemFallback" },
        { "Flour", "Flour" },
        { "Plate", "Plate" },
        { "ShavingKnife", "ShavingKnife" },
        { "Spoon", "Spoon" },
        { "Fork", "Fork" },
        { "Knife", "Knife" },
        { "CuttingBoard", "CuttingBoard" },
        { "Glass", "Glass" },
        { "SoupPan", "SoupPan" },
        { "CookingPan", "CookingPan" },
        { "Wool", "Wool" },
        { "Feather", "Feather" },
        { "Ham", "Ham" },
        { "HamFried", "HamFried" },
        { "HamBurned", "HamBurned" },
        { "Pitchfork", "Pitchfork" },
        { "BucketWithMilk", "BucketWithMilk" },
        { "PickaxeIron", "PickaxeIron" },
        { "Log", "Log" },
        { "ButcheringKnife", "ButcheringKnife" },
        { "AnimalFoodSheep", "SheepFood" },
        { "AnimalFoodCow", "CowFood" },
        { "AnimalFoodPig", "PigFood" },
        { "LargeAxe", "LargeAxe" },
        { "SeedOpuntia", "SeedBagOpuntia" },
        { "SeedSaguaro", "SeedBagSaguaro" },
        { "SeedRaspberry", "SeedBagRaspberry" },
        { "SeedBlueberry", "SeedBagBlueberry" },
        { "Opuntia", "Opuntia" },
        { "Saguaro", "Saguaro" },
        { "Blueberry", "Blueberry" },
        { "Raspberry", "Raspberry" },
        { "Honey", "Honey" },
        { "Boot", "Boot" },
        { "SeaShell1","SeaShell1" },
        { "SeaShell2", "SeaShell2" },
        { "SeaStar", "SeaStar" },
        { "FishBullshark", "Bullshark" },
        { "FishPuffleFish", "PuffleFish" },
        { "FishPuffleFishFried", "PuffleFishFried" },
        { "FishPuffleFishBurned", "PuffleFishBurned" },
        { "FishSardine", "Sardine" },
        { "FishSardineFried", "SardineFried" },
        { "FishSardineBurned", "SardineBurned" },
        { "FishBluegill", "Bluegill" },
        { "FishBluegillFried", "BluegillFried" },
        { "FishBluegillBurned", "BluegillBurned" },
        { "FishStingray", "Stingray" },
        { "FishStingrayFried", "StingrayFried" },
        { "FishStingrayBurned", "StingrayBurned" },
        { "RedLure", "RedLure" },
        { "YellowLure", "YellowLure" },
    };

    public string GetItemIdByOldPrefabName(string oldPrefabName)
    {
        if (oldPrefabToIdMapper.ContainsKey(oldPrefabName))
        {
            return oldPrefabToIdMapper[oldPrefabName];
        }

        return "";
    }

    public SaveableItem GetUpdatedItem(SaveableItem item)
    {
        if (item == null)
        {
            return null;
        }

        if(item.prefabFileName == null)
        {
            return null;
        }

        string id = GetItemIdByOldPrefabName(item.prefabFileName);
        if (id == "")
        {
            throw new System.Exception("no mapping found for prefab: " + item.prefabFileName);
        }

        return new SaveableItem()
        {
            id = id,
            currentStackSize = item.currentStackSize
        };
    }
}