using UnityEngine;

public class HealthIndicator : MonoBehaviour
{
    public GameObject heart;
    public Transform heartParent;

    public void setNumberOfHearts(int numberOfHearts)
    {
        foreach (Transform child in heartParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < numberOfHearts; i++)
        {
            Instantiate(heart, heartParent);
        }
    }
}
