using NUnit.Framework;
using UnityEngine;
using System.Linq;
using HarvestDataTypes;
using UnityEditor;
using BNG;

namespace Items
{
    /// <summary>
    /// TEMPORARY one-shot repair tests. Each test here walks every item prefab
    /// and mutates the asset on disk to satisfy the matching validity test in
    /// <see cref="ItemValidityTest"/>. They are written as NUnit tests purely
    /// to piggyback on the existing Test Runner workflow and database setup.
    ///
    /// Usage:
    ///   1. Run the desired fixer from Window ▸ General ▸ Test Runner (EditMode).
    ///   2. Read the Console summary and commit the resulting prefab diffs.
    ///   3. Re-run the real regression tests in <see cref="ItemValidityTest"/>
    ///      to confirm everything is green.
    ///   4. When no longer needed, delete this entire file.
    ///
    /// Important: do not add long-lived tests here. Anything that should run
    /// on every CI build belongs in <see cref="ItemValidityTest"/>.
    /// </summary>
    public class ItemPrefabFixerTest
    {
        GameObject databaseManager;
        ItemDatabase itemDatabase;

        [SetUp]
        public void SetUp()
        {
            StartDatabaseManager();
            GameState.Reset();
        }

        [TearDown]
        public void Cleanup()
        {
            GameState.Reset();
            Object.DestroyImmediate(databaseManager);
        }

        // Fixer for `EveryItemPrefabRootHasItemInformationPointingAtItself`.
        // Adds a missing ItemInformation component and/or repoints a
        // missing/incorrect `item` reference on every item prefab.
        [Test]
        public void TEMPORARY_FixEveryItemPrefabItemInformation()
        {
            int added = 0;
            int repointed = 0;

            foreach (HarvestDataTypes.Item item in itemDatabase.items)
            {
                if (item.prefab == null)
                {
                    continue;
                }

                string assetPath = AssetDatabase.GetAssetPath(item.prefab);
                if (string.IsNullOrEmpty(assetPath))
                {
                    Debug.LogWarning(
                        $"[ItemPrefabFixer:ItemInformation] Item '{item.itemId}' has a prefab that is not an asset on disk — skipping."
                    );
                    continue;
                }

                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
                try
                {
                    bool changed = false;

                    ItemInformation itemInformation = prefabRoot.GetComponent<ItemInformation>();
                    if (itemInformation == null)
                    {
                        itemInformation = prefabRoot.AddComponent<ItemInformation>();
                        added++;
                        changed = true;
                        Debug.Log($"[ItemPrefabFixer:ItemInformation] Added missing ItemInformation to prefab for '{item.itemId}'.");
                    }

                    if (itemInformation.item != item)
                    {
                        string before = itemInformation.item != null ? itemInformation.item.itemId : "<null>";
                        itemInformation.item = item;
                        repointed++;
                        changed = true;
                        Debug.Log($"[ItemPrefabFixer:ItemInformation] Fixed ItemInformation.item on '{item.itemId}' (was '{before}').");
                    }

                    if (changed)
                    {
                        PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
                    }
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log(
                $"[ItemPrefabFixer:ItemInformation] Done. Added ItemInformation to {added} prefabs, " +
                $"repointed item on {repointed} prefabs."
            );
        }

        // Fixer for `ItHasCorrectGrabbableOptions`. Sets the canonical Grabbable
        // configuration on every item prefab's root Grabbable. Skips the same
        // exception list as the validity test (two-handed items, etc.).
        [Test]
        public void TEMPORARY_FixEveryItemPrefabGrabbableOptions()
        {
            // Keep in sync with the exclusion list in
            // ItemValidityTest.ItHasCorrectGrabbableOptions.
            string[] listOfWeirdItems = {
                "Wallet",
                "Basket",
                "FishingRod",
                "Hammer",
                "LargeAxe",
                "Pickaxe",
                "PickaxeIron",
            };

            int fixedPrefabs = 0;
            int totalFieldChanges = 0;

            foreach (HarvestDataTypes.Item item in itemDatabase.items)
            {
                if (item.prefab == null)
                {
                    continue;
                }

                if (listOfWeirdItems.Contains(item.itemId))
                {
                    continue;
                }

                string assetPath = AssetDatabase.GetAssetPath(item.prefab);
                if (string.IsNullOrEmpty(assetPath))
                {
                    Debug.LogWarning(
                        $"[ItemPrefabFixer:Grabbable] Item '{item.itemId}' has a prefab that is not an asset on disk — skipping."
                    );
                    continue;
                }

                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
                try
                {
                    Grabbable grabbable = prefabRoot.GetComponent<Grabbable>();
                    if (grabbable == null)
                    {
                        Debug.LogWarning(
                            $"[ItemPrefabFixer:Grabbable] Prefab for '{item.itemId}' has no Grabbable on root — skipping."
                        );
                        continue;
                    }

                    int changes = 0;

                    if (grabbable.GrabPhysics != GrabPhysics.PhysicsJoint)
                    {
                        Debug.Log(
                            $"[ItemPrefabFixer:Grabbable] '{item.itemId}'.GrabPhysics: " +
                            $"{grabbable.GrabPhysics} → PhysicsJoint"
                        );
                        grabbable.GrabPhysics = GrabPhysics.PhysicsJoint;
                        changes++;
                    }

                    if (!grabbable.ParentHandModel)
                    {
                        Debug.Log($"[ItemPrefabFixer:Grabbable] '{item.itemId}'.ParentHandModel: false → true");
                        grabbable.ParentHandModel = true;
                        changes++;
                    }

                    if (grabbable.GrabSpeed != 20)
                    {
                        Debug.Log($"[ItemPrefabFixer:Grabbable] '{item.itemId}'.GrabSpeed: {grabbable.GrabSpeed} → 20");
                        grabbable.GrabSpeed = 20;
                        changes++;
                    }

                    // RemoteGrabDistance is a float; allow a tiny epsilon around 2.5f.
                    if (!Mathf.Approximately(grabbable.RemoteGrabDistance, 2.5f))
                    {
                        Debug.Log(
                            $"[ItemPrefabFixer:Grabbable] '{item.itemId}'.RemoteGrabDistance: " +
                            $"{grabbable.RemoteGrabDistance} → 2.5"
                        );
                        grabbable.RemoteGrabDistance = 2.5f;
                        changes++;
                    }

                    if (changes > 0)
                    {
                        PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
                        fixedPrefabs++;
                        totalFieldChanges += changes;
                    }
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log(
                $"[ItemPrefabFixer:Grabbable] Done. Adjusted {fixedPrefabs} prefabs " +
                $"({totalFieldChanges} field changes total)."
            );
        }

        void StartDatabaseManager()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/_Core/DatabaseManager/DatabaseManager.prefab");
            databaseManager = GameObject.Instantiate(prefab);
            itemDatabase = databaseManager.GetComponent<DatabaseManager>().items;
        }
    }
}
