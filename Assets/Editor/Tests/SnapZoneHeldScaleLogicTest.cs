using BNG;
using NUnit.Framework;

namespace Tests
{
    public class SnapZoneHeldScaleLogicTest
    {
        [Test]
        public void ResolveMultiplier_UsesZoneScaleItemWhenNoItemScale()
        {
            float result = SnapZoneHeldScaleLogic.ResolveMultiplier(
                zoneScaleItem: 0.5f,
                useZoneScaleOnly: false,
                itemScale: null);

            Assert.AreEqual(0.5f, result);
        }

        [Test]
        public void ResolveMultiplier_MultipliesZoneAndItemScaleWhenBothPresent()
        {
            float result = SnapZoneHeldScaleLogic.ResolveMultiplier(
                zoneScaleItem: 0.5f,
                useZoneScaleOnly: false,
                itemScale: 0.25f);

            Assert.AreEqual(0.125f, result);
        }

        [Test]
        public void ResolveMultiplier_ZoneScaleCompensatesItemScaleForFullSize()
        {
            float result = SnapZoneHeldScaleLogic.ResolveMultiplier(
                zoneScaleItem: 4f,
                useZoneScaleOnly: false,
                itemScale: 0.25f);

            Assert.AreEqual(1f, result);
        }

        [Test]
        public void ResolveMultiplier_UsesZoneScaleItemWhenUseZoneScaleOnly()
        {
            float result = SnapZoneHeldScaleLogic.ResolveMultiplier(
                zoneScaleItem: 1f,
                useZoneScaleOnly: true,
                itemScale: 0.25f);

            Assert.AreEqual(1f, result);
        }
    }
}
