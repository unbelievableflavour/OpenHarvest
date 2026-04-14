using UnityEngine;

public class RenderOnCollide : MonoBehaviour
{
    public GameObject objectToRender;

    private void Start()
    {
        objectToRender.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player")
        {
            return;
        }

        objectToRender.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        objectToRender.SetActive(false);
    }
}
