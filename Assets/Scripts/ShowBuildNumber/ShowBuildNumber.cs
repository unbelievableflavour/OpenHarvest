using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowBuildNumber : MonoBehaviour
{
    public Text label;
    // Start is called before the first frame update
    void Start()
    {
        label.text = "Build: " + Application.version;
    }
}
