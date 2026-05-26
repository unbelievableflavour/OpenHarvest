namespace BNG {
    /// <summary>
    /// Resolves held-item scale when snapped. <see cref="SnapZone.ScaleItem"/> is always the base.
    /// <see cref="SnapZoneScale"/> on the grabbable multiplies that base unless <see cref="SnapZone.UseZoneScaleOnly"/>.
    /// </summary>
    public static class SnapZoneHeldScaleLogic {
        public static float ResolveMultiplier(float zoneScaleItem, bool useZoneScaleOnly, float? itemScale) {
            if (useZoneScaleOnly || !itemScale.HasValue) {
                return zoneScaleItem;
            }

            return zoneScaleItem * itemScale.Value;
        }
    }
}
