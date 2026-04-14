using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTestLoader : MonoBehaviour
{
    public int sceneIndex = 0;
    void Start()
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
