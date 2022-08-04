using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchToScene : MonoBehaviour
{
    public int sceneIndex;
    public string sceneEnterLocationName;

    public void Switch()
    {
        GameState.currentSceneSwitcher.SwitchToScene(sceneIndex, sceneEnterLocationName);
    }

    public void SwitchWithPreviousSceneNumber()
    {
        GameState.currentSceneSwitcher.SwitchToScene(sceneIndex, SceneManager.GetActiveScene().buildIndex.ToString());
    }
}
