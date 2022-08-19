using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;
using System.Linq;
using HarvestDataTypes;
using UnityEditor;
using OVRSimpleJSON;
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
        // public void ItCanLoadAllItemsIntoTheItemDatabase()
        // {
        //     Assert.AreEqual(true, itemCountIsHigherThan200());
        // }

        // [Test]
        // public void ItChecksAllItemsForValidity()
        // {
        //     Assert.AreEqual(true, itemCountIsHigherThan200());

        //     foreach (var item in Definitions.ItemsWithInformation)
        //     {
        //         if (item.Value.maximumTimesOwned != null)
        //         {
        //             Assert.AreEqual(GameState.Instance.unlockables.ContainsKey(item.Value.itemId), true, "item: \"" + item.Value.itemId + "\" is invalid. Property \"isUnlockable\" is required to use property \"maximumTimesOwned\"");
        //         }

        //         Assert.AreEqual(false, string.IsNullOrEmpty(item.Value.name), "item: \"" + item.Value.itemId + "\" is invalid. Title value is empty");
        //     }
        // }

        // [Test]
        // public void ItCanInstantiateEveryItem()
        // {
        //     Assert.AreEqual(true, itemCountIsHigherThan200());

        //     foreach (var item in Definitions.ItemsWithInformation)
        //     {
        //         if (item.Value.prefab == null)
        //         {
        //             continue;
        //         }

        //         GameObject spawnedItem = Definitions.InstantiateItemNew(item.Value.prefab);

        //         Assert.AreEqual(false, spawnedItem.GetComponents<GrabbableHaptics>().Length == 2, spawnedItem.gameObject.name + " has 2 grabbableHaptics components");

        //         Object.DestroyImmediate(spawnedItem);
        //     }

        //     foreach (HarvestDataTypes.Item item in DatabaseManager.Instance.items.items)
        //     {
        //         if (item.prefab == null)
        //         {
        //             continue;
        //         }

        //         GameObject spawnedItem = Definitions.InstantiateItemNew(item.prefab);
        //         Object.DestroyImmediate(spawnedItem);
        //     }
        // }

        // [Test]
        // public void ItHasScale1AndRot0AndPos0ForPrefabItems()
        // {
        //     Assert.AreEqual(true, itemCountIsHigherThan200());

        //     foreach (var item in Definitions.ItemsWithInformation)
        //     {
        //         if (item.Value.prefab == null)
        //         {
        //             continue;
        //         }

        //         GameObject spawnedItem = Definitions.InstantiateItemNew(item.Value.prefab);
        //         Assert.AreEqual(0 , spawnedItem.transform.position.x, "Invalid position X for item: " + spawnedItem.transform.name);
        //         Assert.AreEqual(0, spawnedItem.transform.position.y, "Invalid position Y for item: " + spawnedItem.transform.name);
        //         Assert.AreEqual(0, spawnedItem.transform.position.z, "Invalid position Z for item: " + spawnedItem.transform.name);
        //         Assert.AreEqual(0, spawnedItem.transform.rotation.x, "Invalid rotation X for item: " + spawnedItem.transform.name);
        //         Assert.AreEqual(0, spawnedItem.transform.rotation.y, "Invalid rotation Y for item: " + spawnedItem.transform.name);
        //         Assert.AreEqual(0, spawnedItem.transform.rotation.z, "Invalid rotation Z for item: " + spawnedItem.transform.name);
        //         Assert.AreEqual(1.0f, spawnedItem.transform.localScale.x, "Invalid scale X for item: " + spawnedItem.transform.name);
        //         Assert.AreEqual(1.0f, spawnedItem.transform.localScale.y, "Invalid scale Y for item: " + spawnedItem.transform.name);
        //         Assert.AreEqual(1.0f, spawnedItem.transform.localScale.z, "Invalid scale Z for item: " + spawnedItem.transform.name);
        //         Object.DestroyImmediate(spawnedItem);
        //     }
        // }

        // [Test]
        // public void ItHasCorrectRigidBodyOptions()
        // {
        //     Assert.AreEqual(true, itemCountIsHigherThan200());

        //     foreach (var item in Definitions.ItemsWithInformation)
        //     {
        //         if (item.Value.prefab == null)
        //         {
        //             continue;
        //         }

        //         //Weird wallet, skip for now
        //         if(item.Value.name == "Wallet"){
        //             continue;
        //         }

        //         GameObject spawnedItem = Definitions.InstantiateItemNew(item.Value.prefab);
        //         Rigidbody rigidBody = spawnedItem.GetComponent<Rigidbody>();
        //         Assert.AreEqual(CollisionDetectionMode.ContinuousDynamic, rigidBody.collisionDetectionMode, "Invalid rigidBody collision detection for item: " + spawnedItem.transform.name);
        //         Object.DestroyImmediate(spawnedItem);
        //     }
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
