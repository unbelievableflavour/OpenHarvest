using UnityEngine;
using UnityEngine.SceneManagement;

public class HideIfNotInScene : MonoBehaviour
{
    public int SceneIndex;

    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != SceneIndex)
        {
            this.gameObject.SetActive(false);
            return;
        }
    }
}
