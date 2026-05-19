using UnityEngine;

public class RandomAnimationSpeed : MonoBehaviour
{
    public float minSpeed = 2;
    public float maxSpeed = 4;
    void Start()
    {
        GetComponent<Animator>().speed = Random.Range(minSpeed, maxSpeed);
    }
}
