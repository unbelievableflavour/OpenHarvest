using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchToScene : MonoBehaviour
{
    public int sceneIndex;
    public string sceneEnterLocationName;

    public void Switch()
    {
        SceneSwitcher.Instance.SwitchToScene(sceneIndex, sceneEnterLocationName);
    }

    public void SwitchWithPreviousSceneNumber()
    {
        SceneSwitcher.Instance.SwitchToScene(sceneIndex, SceneManager.GetActiveScene().buildIndex.ToString());
    }
}
