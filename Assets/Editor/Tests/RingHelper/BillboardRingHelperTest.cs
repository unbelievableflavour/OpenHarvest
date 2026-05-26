using NUnit.Framework;

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
        public void GetWorldHalfExtent_UsesMultiplierWhenTargeted()
        {
            float result = BillboardRingHelperLogic.GetWorldHalfExtent(
                baseExtent: 0.1f,
                isTargetedGrabbable: true);

            Assert.AreEqual(0.112f, result, 0.0001f);
        }

        [Test]
        public void GetWorldHalfExtent_UsesBaseWhenNotTargeted()
        {
            float result = BillboardRingHelperLogic.GetWorldHalfExtent(
                baseExtent: 0.1f,
                isTargetedGrabbable: false);

            Assert.AreEqual(0.1f, result, 0.0001f);
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
    }
}
