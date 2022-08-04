using NUnit.Framework;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class SceneInformationManagerTest
    {
        [Test]
        public void ItChecksIfTheSceneCountIsEqualToActualSceneCount()
        {
            Assert.AreEqual(SceneInformationManager.getSceneCount(), SceneManager.sceneCountInBuildSettings);
        }

        [Test]
        public void ItCanRetrieveTheSceneDetailsByIndex()
        {
            Assert.AreEqual(true, SceneInformationManager.getSceneDetailsByIndex(1).usesWeather);
            Assert.AreEqual(false, SceneInformationManager.getSceneDetailsByIndex(4).usesWeather);
        }
    }
}
