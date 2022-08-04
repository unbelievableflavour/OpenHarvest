using UnityEngine;

public class SwitchSceneOnCollision : MonoBehaviour
{
    public int sceneIndex;
    public string sceneEnterLocationName;

    private bool canCollide = true;

    void OnTriggerEnter(Collider col)
    {
        if (!canCollide)
        {
            return;
        }

        if (col.tag != "Face")
        {
            return;
        }

        GameState.currentSceneSwitcher.SwitchToScene(sceneIndex, sceneEnterLocationName);
        canCollide = false;
    }
}
