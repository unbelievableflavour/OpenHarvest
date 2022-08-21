using BNG;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Player
{
    public class PlayerTest
    {
        GameObject player;
        Transform xrRigAdvanced;

        [SetUp]
        public void SetUp()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Components/Player/NewCustomPlayerAdvanced Variant.prefab");
            player = GameObject.Instantiate(prefab);
            xrRigAdvanced = player.transform.Find("XR Rig Advanced");
        }

        [Test]
        public void ItChecksIfAllRequiredFieldsAreNotEmpty()
        {
            var playerController = xrRigAdvanced.GetComponentInChildren<BNGPlayerController>();

            Assert.AreEqual(0, playerController.CharacterControllerYOffset);

            var playerRotation = xrRigAdvanced.GetComponentInChildren<PlayerRotation>();

            Assert.AreEqual(100, playerRotation.SmoothTurnSpeed);

            var playerTeleport = xrRigAdvanced.GetComponentInChildren<PlayerTeleport>();

            Assert.AreEqual(45, playerTeleport.MaxSlope);
        }

        [Test]
        public void ItChecksIfSprintingIsConfigured()
        {
            var smoothLocomotion = xrRigAdvanced.GetComponentInChildren<SmoothLocomotion>();

            Assert.AreEqual(5, smoothLocomotion.SprintSpeed);
            Assert.AreEqual("LeftThumbstick", smoothLocomotion.SprintInput[0].ToString());
        }

        [Test]
        public void ItChecksIfCustomHandsAreConfigured()
        {
            var handModelSelector = xrRigAdvanced.GetComponentInChildren<HandModelSelector>();

            Assert.AreEqual(8, handModelSelector.DefaultHandsModel);
            Assert.AreEqual(ControllerBinding.None, handModelSelector.ToggleHandsInput);
            var trackingSpace = xrRigAdvanced.transform.Find("PlayerController/CameraRig/TrackingSpace");

            var leftHandModels = trackingSpace.transform.Find("LeftHandAnchor/LeftControllerAnchor/LeftController/ModelsLeft");
            var defaultLeftHand = leftHandModels.transform.Find("CustomHandLeftBlackNew");
            var customleftIKHand = leftHandModels.transform.Find("CustomHandLeftIKFarm");
            var customleftHand = leftHandModels.transform.Find("CustomHandLeftFarm");

            Assert.AreEqual(false, defaultLeftHand.gameObject.activeSelf);
            Assert.AreEqual(true, customleftIKHand.gameObject.activeSelf);
            Assert.AreEqual(false, customleftHand.gameObject.activeSelf);

            var rightHandModels = trackingSpace.transform.Find("RightHandAnchor/RightControllerAnchor/RightController/ModelsRight");
            var defaultRightHand = rightHandModels.transform.Find("CustomHandRightBlackNew");
            var customrightIKHand = rightHandModels.transform.Find("CustomHandRightIKFarm");
            var customrightHand = rightHandModels.transform.Find("CustomHandRightFarm");

            Assert.AreEqual(false, defaultRightHand.gameObject.activeSelf);
            Assert.AreEqual(true, customrightIKHand.gameObject.activeSelf);
            Assert.AreEqual(false, customrightHand.gameObject.activeSelf);
        }

        [Test]
        public void ItChecksIfCustomBodyIsConfigured()
        {
            var customBody = xrRigAdvanced.transform.Find("BodyIK/BodyOnly/Body");

            Assert.AreNotEqual(null, customBody);
        }

        [Test]
        public void ItChecksIfLocomotionToggleByButtonIsDisabled()
        {
            var locomotionManager = xrRigAdvanced.GetComponentInChildren<LocomotionManager>();

            Assert.AreEqual(0, locomotionManager.locomotionToggleInput.Count);
            Assert.AreEqual(null, locomotionManager.LocomotionToggleAction);
        }

        [Test]
        public void ItChecksIfPlayerHeightOffsetIsConfigured()
        {
            Assert.AreNotEqual(false, xrRigAdvanced.GetComponent<PlayerHeightOffset>());
        }

        [Test]
        public void ItChecksIfPlayerColoursIsConfigured()
        {
            Assert.AreNotEqual(false, xrRigAdvanced.GetComponent<PlayerColours>());
        }

        [Test]
        public void ItChecksIfAllPlayerCollidersAreIgnored()
        {
            var headCollisionFade = xrRigAdvanced.GetComponentInChildren<HeadCollisionFade>();
            var ignoreColliders = headCollisionFade.GetComponent<IgnoreColliders>();

            Assert.AreEqual(true, ignoreColliders.CollidersToIgnore.Count == 6);
            Assert.AreEqual(true, ignoreColliders.CollidersToIgnore[0].transform.name == "PlayerController");
            Assert.AreEqual(true, ignoreColliders.CollidersToIgnore[1].transform.name == "mouthCollider");
            Assert.AreEqual(true, ignoreColliders.CollidersToIgnore[2].transform.name == "HeadInventorySlot");
            Assert.AreEqual(true, ignoreColliders.CollidersToIgnore[3].transform.name == "CenterShoulder");
            Assert.AreEqual(true, ignoreColliders.CollidersToIgnore[4].transform.name == "DropSlot");
            Assert.AreEqual(true, ignoreColliders.CollidersToIgnore[5].transform.name == "BreastPocket");
        }

        [Test]
        public void ItChecksIfTheConsoleComponentIsConfigured()
        {
            var consoleComp = xrRigAdvanced.GetComponentInChildren<EnableConsole>();

            Assert.AreNotEqual(null, consoleComp);
        }


        [Test]
        public void ItChecksIfThePauseMenuIsDisabled()
        {
            var pauseMenu = xrRigAdvanced.GetComponentInChildren<PauseMenuController>();

            Assert.AreNotEqual(null, pauseMenu);
        }

        [Test]
        public void ItChecksIfMenuToggleIsConfigured()
        {
            var locomotionManager = xrRigAdvanced.GetComponentInChildren<LocomotionManager>();

            Assert.AreEqual(0, locomotionManager.locomotionToggleInput.Count);
            Assert.AreEqual(null, locomotionManager.LocomotionToggleAction);
        }

        [Test]
        public void ItChecksIfAllSlowMotionIsDisabled()
        {
            var timeController = xrRigAdvanced.GetComponentInChildren<BNG.TimeController>();

            Assert.AreEqual(false, timeController.YKeySlowsTime);
            Assert.AreEqual(null, timeController.SlowTimeAction);
        }

        [Test]
        public void ItChecksIfElevationCheckerIsConfigured()
        {
            var playerController = xrRigAdvanced.GetComponentInChildren<BNGPlayerController>();

            Assert.AreEqual(0, playerController.MinElevation);
            Assert.AreEqual(0, playerController.MaxElevation);

            var elevationChecker = xrRigAdvanced.GetComponent<ElevationChecker>();

            Assert.AreEqual(true, elevationChecker.enabled);
            Assert.AreEqual(-100.0f, elevationChecker.MinElevation);
            Assert.AreEqual(5000.0f, elevationChecker.MaxElevation);
        }

        [Test]
        public void ItChecksIfRemoteGrabberHasBeenWidened()
        {
            var remoteGrabbers = xrRigAdvanced.GetComponentsInChildren<RemoteGrabber>();
            var leftHandcolliders = remoteGrabbers[0].GetComponents<CapsuleCollider>();

            Assert.AreEqual(0.08f, leftHandcolliders[0].radius);
            Assert.AreEqual(0.15f, leftHandcolliders[1].radius);
            Assert.AreEqual(0.5f, leftHandcolliders[2].radius);

            var rightHandcolliders = remoteGrabbers[1].GetComponents<CapsuleCollider>();

            Assert.AreEqual(0.08f, rightHandcolliders[0].radius);
            Assert.AreEqual(0.15f, rightHandcolliders[1].radius);
            Assert.AreEqual(0.5f, rightHandcolliders[2].radius);
        }

        [Test]
        public void ItChecksIfTheCustomPointerIsConfigured()
        {
            var uiPointers = xrRigAdvanced.GetComponentsInChildren<XRInteractorLineVisual>(true);

            Assert.AreEqual("CustomXRCursorLeft", uiPointers[0].reticle.transform.name);
            Assert.AreEqual("CustomXRCursorRight", uiPointers[1].reticle.transform.name);
        }

        [Test]
        public void ItChecksIfXRITIsConfigured()
        {
            var xRUIInputModule = player.GetComponentsInChildren<XRUIInputModule>();

            Assert.AreNotEqual(null, xRUIInputModule);
        }

        [TearDown]
        public void Cleanup()
        {
            xrRigAdvanced = null;
            Object.DestroyImmediate(player);
        }
    }
}