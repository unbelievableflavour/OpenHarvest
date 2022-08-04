using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using static Definitions;

namespace Tests
{
    public class PatchTest
    {
        SoilBehaviour soilBehaviour;
        PlantManager plantManager;
        GameObject testGameObject;
        GameObject audioManager;

        [SetUp]
        public void SetUp()
        {
            var audioManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/AudioManager/AudioManager.prefab");
            audioManager = GameObject.Instantiate(audioManagerPrefab);
            AudioManager.Instance = audioManager.GetComponent<AudioManager>();

            var patchPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Patch/Patch.prefab");
            var patch = GameObject.Instantiate(patchPrefab);
            soilBehaviour = patch.GetComponent<SoilBehaviour>();
            soilBehaviour.m_GrassTile = new GameObject();
            soilBehaviour.m_ShoveledTile = new GameObject();
            soilBehaviour.m_PlowedTile = new GameObject();

            plantManager = soilBehaviour.plantManager;
            plantManager.Awake();
            testGameObject = new GameObject("test");
        }

        [Test]
        public void ItCanBeInitialized()
        {
            soilBehaviour.Reset();

            Assert.AreEqual(SoilStates.Initialized, soilBehaviour.getCurrentState());
            Assert.AreEqual(true, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_PlowedTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.getWatered());
            Assert.AreEqual("", plantManager.getSelectedPlant());
        }

        [Test]
        public void ItCanBeShoveledWhenInitialized()
        {
            soilBehaviour.Reset();

            Assert.AreEqual(SoilStates.Initialized, soilBehaviour.getCurrentState());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Shovel";

            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.Shoveled, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_PlowedTile.activeSelf);
        }

        [Test]
        public void ItCanBeShoveledWhenShoveled()
        {
            soilBehaviour.Reset();
            soilBehaviour.setCurrentState(SoilStates.Plowed);

            Assert.AreEqual(SoilStates.Plowed, soilBehaviour.getCurrentState());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Shovel";

            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.Shoveled, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_PlowedTile.activeSelf);
        }

        [Test]
        public void ItCanBeShoveledWhenPlowed()
        {
            soilBehaviour.Reset();
            soilBehaviour.setCurrentState(SoilStates.Plowed);

            Assert.AreEqual(SoilStates.Plowed, soilBehaviour.getCurrentState());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Shovel";

            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.Shoveled, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_PlowedTile.activeSelf);
        }

        /*
        INITIALIZED CANNOT BE MADE WATERED YET

        [Test]
        public void ItCanBeWateredWhenInitialized()
        {
            soilBehaviour.Reset();

            Assert.AreEqual(SoilStates.Initialized, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.getWatered());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Water";

            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.Initialized, soilBehaviour.getCurrentState());
            Assert.AreEqual(true, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_PlowedTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.getWatered());
        }
        */

        [Test]
        public void ItCanBeWateredWhenShoveled()
        {
            soilBehaviour.Reset();
            soilBehaviour.setCurrentState(SoilStates.Shoveled);

            Assert.AreEqual(SoilStates.Shoveled, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.getWatered());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Water";

            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.Shoveled, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_PlowedTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.getWatered());
        }

        [Test]
        public void ItCanBeWateredWhenWatered()
        {
            soilBehaviour.Reset();
            soilBehaviour.setCurrentState(SoilStates.Shoveled);

            Assert.AreEqual(SoilStates.Shoveled, soilBehaviour.getCurrentState());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Water";

            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(true, soilBehaviour.getWatered());

            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.Shoveled, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_PlowedTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.getWatered());
        }

        [Test]
        public void ItCanBeWateredWhenPlowed()
        {
            soilBehaviour.Reset();
            soilBehaviour.setCurrentState(SoilStates.Plowed);

            Assert.AreEqual(SoilStates.Plowed, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.getWatered());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Water";

            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.Plowed, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.m_PlowedTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.getWatered());
        }

        [Test]
        public void ItNeedsToBeShoveled4TimesBeforeMovingOn()
        {
            soilBehaviour.Reset();

            Assert.AreEqual(SoilStates.Initialized, soilBehaviour.getCurrentState());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Shovel";

            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.Initialized, soilBehaviour.getCurrentState());
            Assert.AreEqual(true, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_PlowedTile.activeSelf);
        }

        [Test]
        public void ItNeedsToBePlowed4TimesBeforeMovingOn()
        {
            soilBehaviour.Reset();
            soilBehaviour.setCurrentState(SoilStates.Shoveled);

            Assert.AreEqual(SoilStates.Shoveled, soilBehaviour.getCurrentState());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Hoe";

            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.Shoveled, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_PlowedTile.activeSelf);
        }

        [Test]
        public void ItCanNotBePlowedWhenInitialized()
        {
            soilBehaviour.Reset();

            Assert.AreEqual(SoilStates.Initialized, soilBehaviour.getCurrentState());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Hoe";

            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.Initialized, soilBehaviour.getCurrentState());
            Assert.AreEqual(true, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_PlowedTile.activeSelf);
        }

        [Test]
        public void ItCanNotBePlowedWhenInitializedWithObject()
        {
            soilBehaviour.breakableObjectPrefab = new GameObject();
            soilBehaviour.Reset();

            Assert.AreEqual(SoilStates.InitializedWithObject, soilBehaviour.getCurrentState());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Hoe";

            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.InitializedWithObject, soilBehaviour.getCurrentState());
            Assert.AreEqual(true, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_PlowedTile.activeSelf);

            soilBehaviour.breakableObjectPrefab = null;
        }

        [Test]
        public void ItCanNotBeShoveledWhenInitializedWithObject()
        {
            soilBehaviour.breakableObjectPrefab = new GameObject();
            soilBehaviour.Reset();

            Assert.AreEqual(SoilStates.InitializedWithObject, soilBehaviour.getCurrentState());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Shovel";

            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.InitializedWithObject, soilBehaviour.getCurrentState());
            Assert.AreEqual(true, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_PlowedTile.activeSelf);

            soilBehaviour.breakableObjectPrefab = null;
        }

        [Test]
        public void ItCanNotBeWateredWhenInitializedWithObject()
        {
            soilBehaviour.breakableObjectPrefab = new GameObject();
            soilBehaviour.Reset();

            Assert.AreEqual(SoilStates.InitializedWithObject, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.getWatered());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Water";

            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.InitializedWithObject, soilBehaviour.getCurrentState());
            Assert.AreEqual(true, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_PlowedTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.getWatered());

            soilBehaviour.breakableObjectPrefab = null;
        }

        [Test]
        public void ItCanNotBeSowedWhenInitializedWithObject()
        {
            soilBehaviour.breakableObjectPrefab = new GameObject();
            soilBehaviour.Reset();

            Assert.AreEqual(SoilStates.InitializedWithObject, soilBehaviour.getCurrentState());
            Assert.AreEqual("", plantManager.getSelectedPlant());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Seed";
            TestCollider.name = "Tomato";

            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.InitializedWithObject, soilBehaviour.getCurrentState());
            Assert.AreEqual(true, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_PlowedTile.activeSelf);
            Assert.AreEqual("", plantManager.getSelectedPlant());

            soilBehaviour.breakableObjectPrefab = null;
        }

        [Test]
        public void ItCanBePlowedWhenShoveled()
        {
            soilBehaviour.Reset();
            soilBehaviour.setCurrentState(SoilStates.Shoveled);

            Assert.AreEqual(SoilStates.Shoveled, soilBehaviour.getCurrentState());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Hoe";

            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.Plowed, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.m_PlowedTile.activeSelf);
        }

        [Test]
        public void ItCanBePlowedWhenPlowed()
        {
            soilBehaviour.Reset();
            soilBehaviour.setCurrentState(SoilStates.Plowed);

            Assert.AreEqual(SoilStates.Plowed, soilBehaviour.getCurrentState());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Hoe";

            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);
            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.Plowed, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.m_PlowedTile.activeSelf);
        }

        [Test]
        public void ItCanBeSowedWhenPlowed()
        {
            soilBehaviour.Reset();
            soilBehaviour.setCurrentState(SoilStates.Plowed);

            Assert.AreEqual(SoilStates.Plowed, soilBehaviour.getCurrentState());
            Assert.AreEqual("", plantManager.getSelectedPlant());
            Assert.AreEqual(0, plantManager.transform.childCount);

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Seed";
            TestCollider.name = "Carrot";

            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.Sowed, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.m_PlowedTile.activeSelf);
            Assert.AreEqual("Carrot", plantManager.getSelectedPlant());
            Assert.AreEqual(1, plantManager.transform.childCount);
        }


        /*
        SHOULD BE RUN IN PLAYMODE
        
        [Test]
        public void ItCanBeShoveledWhenSowed()
        {
            soilBehaviour.Reset();
            soilBehaviour.setCurrentState(SoilStates.Sowed);
            plantManager.SpawnPlant("Carrot");

            Assert.AreEqual(SoilStates.Sowed, soilBehaviour.getCurrentState());
            Assert.AreEqual("Carrot", plantManager.getSelectedPlant());
            Assert.AreEqual(1, plantManager.transform.childCount);

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Shovel";

            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.Shoveled, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_PlowedTile.activeSelf);
            Assert.AreEqual("", plantManager.getSelectedPlant());
            Assert.AreEqual(0, plantManager.transform.childCount);
        }
        */

        [Test]
        public void ItCanNotBeSowedTwice()
        {
            soilBehaviour.Reset();
            soilBehaviour.setCurrentState(SoilStates.Plowed);

            Assert.AreEqual(SoilStates.Plowed, soilBehaviour.getCurrentState());
            Assert.AreEqual("", plantManager.getSelectedPlant());
            Assert.AreEqual(0, plantManager.transform.childCount);

            var TestSeedCollider = testGameObject.AddComponent<BoxCollider>();
            TestSeedCollider.tag = "Seed";
            TestSeedCollider.name = "Carrot";

            soilBehaviour.OnTriggerEnter(TestSeedCollider);

            Assert.AreEqual(SoilStates.Sowed, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.m_PlowedTile.activeSelf);
            Assert.AreEqual("Carrot", plantManager.getSelectedPlant());
            Assert.AreEqual(1, plantManager.transform.childCount);

            soilBehaviour.OnTriggerEnter(TestSeedCollider);

            Assert.AreEqual("Carrot", plantManager.getSelectedPlant());
            Assert.AreEqual(1, plantManager.transform.childCount);
        }

        [Test]
        public void ItCanNotBeSowedTwiceWhenWatered()
        {
            soilBehaviour.Reset();
            soilBehaviour.setCurrentState(SoilStates.Plowed);

            Assert.AreEqual(SoilStates.Plowed, soilBehaviour.getCurrentState());
            Assert.AreEqual("", plantManager.getSelectedPlant());
            Assert.AreEqual(0, plantManager.transform.childCount);
            Assert.AreEqual(false, soilBehaviour.getWatered());

            var TestWaterCollider = testGameObject.AddComponent<BoxCollider>();
            TestWaterCollider.tag = "Water";

            soilBehaviour.OnTriggerEnter(TestWaterCollider);

            Assert.AreEqual(true, soilBehaviour.getWatered());

            //Debug.Log(plantManager.getSelectedPlantGrowthState());
            //.Awake();

            var TestSeedCollider = testGameObject.AddComponent<BoxCollider>();
            TestSeedCollider.tag = "Seed";
            TestSeedCollider.name = "Carrot";

            soilBehaviour.OnTriggerEnter(TestSeedCollider);

            //Assert.AreEqual(SoilStates.Sowed, soilBehaviour.getCurrentState());
            //Assert.AreEqual(false, soilBehaviour.m_GrassTile.activeSelf);
            //Assert.AreEqual(false, soilBehaviour.m_ShoveledTile.activeSelf);
            //Assert.AreEqual(true, soilBehaviour.m_PlowedTile.activeSelf);
            //Assert.AreEqual("Carrot", plantManager.getSelectedPlant());
            //Assert.AreEqual(1, plantManager.transform.childCount);

            //soilBehaviour.OnTriggerEnter(TestSeedCollider);

            //Assert.AreEqual("Carrot", plantManager.getSelectedPlant());
            //Assert.AreEqual(1, plantManager.transform.childCount);
        }

        [Test]
        public void ItCanNotBeSowedWhenInitialized()
        {
            soilBehaviour.Reset();
            soilBehaviour.setCurrentState(SoilStates.Initialized);

            Assert.AreEqual(SoilStates.Initialized, soilBehaviour.getCurrentState());
            Assert.AreEqual("", plantManager.getSelectedPlant());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Seed";
            TestCollider.name = "Carrot";

            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.Initialized, soilBehaviour.getCurrentState());
            Assert.AreEqual(true, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_PlowedTile.activeSelf);
            Assert.AreEqual("", plantManager.getSelectedPlant());
        }

        [Test]
        public void ItCanNotBeSowedWhenShoveled()
        {
            soilBehaviour.Reset();
            soilBehaviour.setCurrentState(SoilStates.Shoveled);

            Assert.AreEqual(SoilStates.Shoveled, soilBehaviour.getCurrentState());
            Assert.AreEqual("", plantManager.getSelectedPlant());

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Seed";
            TestCollider.name = "Carrot";

            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.Shoveled, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_PlowedTile.activeSelf);
            Assert.AreEqual("", plantManager.getSelectedPlant());
        }

        [Test]
        public void ItCanNotBeSowedWhenSowed()
        {
            soilBehaviour.Reset();
            soilBehaviour.setCurrentState(SoilStates.Sowed);

            Assert.AreEqual(SoilStates.Sowed, soilBehaviour.getCurrentState());
            Assert.AreEqual("", plantManager.getSelectedPlant());
            Assert.AreEqual(0, plantManager.transform.childCount);

            var TestCollider = testGameObject.AddComponent<BoxCollider>();
            TestCollider.tag = "Seed";
            TestCollider.name = "Carrot";

            soilBehaviour.OnTriggerEnter(TestCollider);

            Assert.AreEqual(SoilStates.Sowed, soilBehaviour.getCurrentState());
            Assert.AreEqual(false, soilBehaviour.m_GrassTile.activeSelf);
            Assert.AreEqual(false, soilBehaviour.m_ShoveledTile.activeSelf);
            Assert.AreEqual(true, soilBehaviour.m_PlowedTile.activeSelf);
            Assert.AreEqual("", plantManager.getSelectedPlant());
            Assert.AreEqual(0, plantManager.transform.childCount);        
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(testGameObject);
            Object.DestroyImmediate(audioManager);
        }
    }
}