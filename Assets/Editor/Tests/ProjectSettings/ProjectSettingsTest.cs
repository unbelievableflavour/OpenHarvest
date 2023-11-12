using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ProjectSettings
{
    public class ProjectSettingsTest
    {
        [Test]
        public void ItHasTheCorrectCompanyInformationSet()
        {
            Assert.AreEqual("BrothersVR", PlayerSettings.companyName);
            Assert.AreEqual("Harvest VR", PlayerSettings.productName);
        }

        [Test]
        public void ItHasTheURPPipelineConfigured()
        {
            Assert.AreEqual("UniversalRenderPipelineAsset", GraphicsSettings.defaultRenderPipeline.name);
        }

        [Test]
        public void ItHasVulkanSetAsGraphicsAPIForAndroid()
        {
            Assert.AreEqual("Vulkan", PlayerSettings.GetGraphicsAPIs(BuildTarget.Android)[0].ToString());
        }

        [Test]
        public void ItHasDirectXSetAsGraphicsAPIForWindows()
        {
            Assert.AreEqual("Direct3D11", PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows)[0].ToString());
        }

        [Test]
        public void ItHasSRPBatcherDisabled()
        {
            Assert.AreEqual(false, GraphicsSettings.useScriptableRenderPipelineBatching);
        }
    }
}