using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Tests
{
    public class DayNightControllerTest
    {
        private GameObject _controllerGo;
        private GameObject _starsGo;
        private GameObject _moonGo;
        private HarvestSettings _harvestSettings;
        private AudioManager _originalAudioManager;

        [SetUp]
        public void SetUp()
        {
            _originalAudioManager = AudioManager.Instance;
            AudioManager.Instance = null;
        }

        [TearDown]
        public void TearDown()
        {
            AudioManager.Instance = _originalAudioManager;

            if (_controllerGo != null)
            {
                Object.DestroyImmediate(_controllerGo);
                _controllerGo = null;
            }

            if (_starsGo != null)
            {
                Object.DestroyImmediate(_starsGo);
                _starsGo = null;
            }

            if (_moonGo != null)
            {
                Object.DestroyImmediate(_moonGo);
                _moonGo = null;
            }

            if (_harvestSettings != null)
            {
                Object.DestroyImmediate(_harvestSettings);
                _harvestSettings = null;
            }
        }

        [Test]
        public void Start_WithSoundtracksAndNoAudioManager_DoesNotThrow()
        {
            DayNightController controller = CreateController(TimeManipulation.Night);

            Assert.DoesNotThrow(() => InvokePrivate(controller, "Start"));
        }

        [Test]
        public void SetNight_WithSoundtracksAndNoAudioManager_DoesNotThrow()
        {
            DayNightController controller = CreateController(TimeManipulation.None);
            InvokePrivate(controller, "Start");

            Assert.DoesNotThrow(() => InvokePrivate(controller, "SetNight"));
        }

        private DayNightController CreateController(TimeManipulation forcedTime)
        {
            _controllerGo = new GameObject("DayNightControllerTestRoot");
            _controllerGo.AddComponent<Light>();
            DayNightController controller = _controllerGo.AddComponent<DayNightController>();

            _starsGo = GameObject.CreatePrimitive(PrimitiveType.Quad);
            controller.stars = _starsGo.GetComponent<Renderer>();

            _moonGo = new GameObject("Moon", typeof(CanvasRenderer), typeof(Image));
            controller.moon = _moonGo.GetComponent<Image>();

            _harvestSettings = ScriptableObject.CreateInstance<HarvestSettings>();
            _harvestSettings.forceTime = forcedTime;
            controller.harvestSettings = _harvestSettings;

            controller.daySoundTrack = AudioClip.Create("day-test", 10, 1, 44100, false);
            controller.nightSoundtrack = AudioClip.Create("night-test", 10, 1, 44100, false);

            return controller;
        }

        private static void InvokePrivate(object instance, string methodName)
        {
            MethodInfo method = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(method, $"Expected method '{methodName}' to exist.");
            method.Invoke(instance, null);
        }
    }
}
