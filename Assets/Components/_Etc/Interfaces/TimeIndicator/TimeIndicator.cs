using UnityEngine;
using UnityEngine.UI;

public class TimeIndicator : MonoBehaviour
{
    private Text indicator;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        indicator = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer < 1)
        {
            return;
        }
        timer = 0;
        indicator.text = TimeController.getCurrentTime().ToString("HH:mm");
    }
}
