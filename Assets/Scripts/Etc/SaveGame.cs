using System;
using System.Collections.Generic;
using static Definitions;

[Serializable]
public class SaveGame
{
    public int buildNumber = 0;
    public int saveNumber;

    public Dictionary<string, string> settings;
    public Dictionary<string, List<SaveablePatch>> soilGrids;
    public Dictionary<string, List<SaveableItem>> itemStashes;
    public Dictionary<string, List<Animal>> animals;
    public Dictionary<string, ItemOfTheWeek> itemsOfTheWeek;

    public SaveableContractsOfTheWeek contractsOfTheWeek;

    public SaveableWindmill windmill;

    public Dictionary<string, RespawningObject> respawningObjects;
    public Dictionary<Quests, Quest> questList;

    public string name;
    public string farmName;

    public double totalSecondsSpentIngame;
    public double totalSimulatedGameSeconds;
    public bool useRealTime;
    public int currentDay;

    public int money = 0;

    public Dictionary<string, int> unlockables2;
    public Dictionary<string, string> locationConfigurations2;

    public string weatherToday;
    public string weatherTomorrow;

    //DEPRECATED
    public string unlockables;
    public string locationConfigurations;
    public ItemOfTheWeek plantOfTheWeek = new ItemOfTheWeek();
    public ItemOfTheWeek fishOfTheWeek = new ItemOfTheWeek();
    public List<SaveablePatch> patches = new List<SaveablePatch>();
    public List<SaveablePatch> greenhousePatches = new List<SaveablePatch>();
    public List<SaveableItem> itemsStoredInBackpack = new List<SaveableItem>();
    public List<SaveableItem> itemsStoredInBasket = new List<SaveableItem>();
    public List<SaveableItem> itemsStoredInStorageCrate = new List<SaveableItem>();
    public List<SaveableItem> itemsStoredInChristmasTree = new List<SaveableItem>();
    public List<Animal> chickens = new List<Animal>();
    public List<Animal> cows = new List<Animal>();
    public List<Animal> sheep = new List<Animal>();
    public List<Animal> pigs = new List<Animal>();
    public Item currentHat; // is now in ItemStashes
    public bool useHandsOnly;
    public bool useSmoothLocomotion;
    public bool useSmoothTurning;
    public float smoothTurningSensitivity;
    public float playerHeightOffset;
    public bool useAssistMode;
    public float backgroundMusicVolume = 1;
}