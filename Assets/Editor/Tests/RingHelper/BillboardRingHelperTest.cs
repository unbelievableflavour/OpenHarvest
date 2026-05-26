using BNG;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class BillboardRingHelperTest
    {
        [Test]
        public void ShouldShowRing_ReturnsFalseWhenHandsFull()
        {
            bool result = BillboardRingHelperLogic.ShouldShowRing(
                handsFull: true,
                distanceToCamera: 0.5f,
                remoteGrabDistance: 2f);

            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldShowRing_ReturnsFalseWhenOutOfRange()
        {
            bool result = BillboardRingHelperLogic.ShouldShowRing(
                handsFull: false,
                distanceToCamera: 5f,
                remoteGrabDistance: 2f);

            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldShowRing_ReturnsTrueWhenInRangeAndHandsNotFull()
        {
            bool result = BillboardRingHelperLogic.ShouldShowRing(
                handsFull: false,
                distanceToCamera: 1f,
                remoteGrabDistance: 2f);

            Assert.IsTrue(result);
        }

        [Test]
        public void GetDisplayColor_UsesRingColorWhenNotClosest()
        {
            Color ring = Color.yellow;
            Color selected = Color.red;
            Color secondary = Color.blue;

            Color result = BillboardRingHelperLogic.GetDisplayColor(
                isClosestGrabbable: false,
                closestGrabber: null,
                ring,
                selected,
                secondary);

            Assert.AreEqual(ring, result);
        }

        [Test]
        public void GetDisplayColor_UsesSecondaryWhenClosestGrabberIsLeft()
        {
            var grabberObject = new GameObject("LeftGrabber");
            var grabber = grabberObject.AddComponent<Grabber>();
            grabber.HandSide = ControllerHand.Left;

            Color result = BillboardRingHelperLogic.GetDisplayColor(
                isClosestGrabbable: true,
                closestGrabber: grabber,
                Color.white,
                Color.red,
                Color.blue);

            Object.DestroyImmediate(grabberObject);
            Assert.AreEqual(Color.blue, result);
        }

        [Test]
        public void StepFadeOpacity_FadesInTowardTarget()
        {
            float result = BillboardRingHelperLogic.StepFadeOpacity(
                currentOpacity: 0.2f,
                targetOpacity: 1f,
                fadeSpeed: 5f,
                deltaTime: 0.1f,
                fadingIn: true);

            Assert.AreEqual(0.7f, result, 0.0001f);
        }

        [Test]
        public void StepFadeOpacity_FadesOutTowardZero()
        {
            float result = BillboardRingHelperLogic.StepFadeOpacity(
                currentOpacity: 0.5f,
                targetOpacity: 1f,
                fadeSpeed: 5f,
                deltaTime: 0.2f,
                fadingIn: false);

            Assert.AreEqual(0f, result, 0.0001f);
        }

        [Test]
        public void ScaleFromLegacyRingSize_MatchesOldCanvasDefaults()
        {
            float inRange = BillboardRingHelperLogic.ScaleFromLegacyRingSize(1500f);
            float grabbable = BillboardRingHelperLogic.ScaleFromLegacyRingSize(1100f);

            Assert.AreEqual(0.15f, inRange, 0.0001f);
            Assert.Greater(grabbable, inRange);
        }

        [Test]
        public void ScaleFromLegacyRingSize_UsesBaseScaleWhenLegacySizeInvalid()
        {
            float result = BillboardRingHelperLogic.ScaleFromLegacyRingSize(0f);
            Assert.AreEqual(0.15f, result, 0.0001f);
        }
    }
}
