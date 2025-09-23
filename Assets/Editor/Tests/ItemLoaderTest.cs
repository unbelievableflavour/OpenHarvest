using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;
using System.Linq;
using HarvestDataTypes;
using UnityEditor;
using BNG;

namespace Tests
{
    public class ItemLoaderTest
    {
        // GameObject databaseManager;
        // [SetUp]
        // public void SetUp()
        // {
        //     StartDatabaseManager();
        //     var itemLoader = new ItemLoader();
        //     itemLoader.Start();
        // }

        // [Test]
        // public void ItCanLoadItemInformationForEveryItem()
        // {
        //     Assert.AreEqual(true, itemCountIsHigherThan200());

        //     foreach (var itemWithInformation in Definitions.ItemsWithInformation)
        //     {
        //         if (itemWithInformation.Value.prefab == null)
        //         {
        //             continue;
        //         }
        //         GameObject spawnedItem = Definitions.InstantiateItemNew(itemWithInformation.Value.prefab);

        //         var item = Definitions.GetItemFromObject(spawnedItem);
        //         if (item != null)
        //         {
        //             Assert.AreEqual(item.itemId, itemWithInformation.Value.itemId);
        //         }

        //         Object.DestroyImmediate(spawnedItem);
        //     }

        //     foreach (HarvestDataTypes.Item itemInDatabase in DatabaseManager.Instance.items.items)
        //     {
        //         if (itemInDatabase.prefab == null)
        //         {
        //             continue;
        //         }

        //         GameObject spawnedItem = Definitions.InstantiateItemNew(itemInDatabase.prefab);
        //         var item = Definitions.GetItemFromObject(spawnedItem);
        //         Assert.AreEqual(item.itemId, dbItem.itemId);
        //         Object.DestroyImmediate(spawnedItem);
        //     }
        // }

        // [Test]
        // public void ItValidatesAllJSOnFiles()
        // {
        //     var loadedJsonFiles = Resources.LoadAll("/", typeof(TextAsset));

        //     string[] validPropertyNames = { 
        //         "id",
        //         "title",
        //         "prefabFileName",
        //         "description",
        //         "buyPrice",
        //         "sellPrice",
        //         "isUnlockable",
        //         "maximumTimesOwned",
        //         "DependsOnBeforeBuying",
        //         "stores",
        //         "type",
        //         "fishingSpawnLocations"
        //     };

        //     foreach (TextAsset jsonFile in loadedJsonFiles)
        //     {
        //         if (jsonFile.name != "itemInfo")
        //         {
        //             continue;
        //         }

        //         JObject jsonItem = null;

        //         try {
        //             jsonItem = JObject.Parse(jsonFile.text);
        //         } catch {
        //             JSONNode jsonObject = JSON.Parse(jsonFile.text);
        //             throw new System.Exception("Error occured parsing item: "+ jsonObject["id"].Value);
        //         }

        //         foreach (var itemProperty in jsonItem)
        //         {
        //             Assert.AreEqual(true, validPropertyNames.Contains(itemProperty.Key), "Invalid property for item: " + jsonItem["id"] + ". Check itemInfo.json");
        //         }
        //     }
        // }

        // [TearDown]
        // public void Cleanup()
        // {
        //     Definitions.ItemsWithInformation.Clear();
        //     GameState.Instance.unlockables.Clear();

        //     foreach (var store in Definitions.itemStores)
        //     {
        //         Definitions.itemStores[store.Key].Clear();
        //     }

        //     Assert.AreEqual(Definitions.ItemsWithInformation.Count, 0);
        //     Object.DestroyImmediate(databaseManager);
        // }

        // public bool itemCountIsHigherThan200()
        // {
        //     return Definitions.ItemsWithInformation.Count > 200;
        // }
        // void StartDatabaseManager()
        // {
        //     var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ScriptableObjects/Databases/DatabaseManager/DatabaseManager.prefab");
        //     databaseManager = GameObject.Instantiate(prefab);
        //     DatabaseManager.Instance = databaseManager.GetComponent<DatabaseManager>();
        // }
    }
}
