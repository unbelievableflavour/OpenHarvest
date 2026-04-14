using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class ModularWorldGeneratorTest
    {
        [Test]
        public void ItChecksTheRequiredUnitySettingIsTurnedOn()
        {
            Assert.AreEqual(true, Physics.autoSyncTransforms);
        }
    }
}
