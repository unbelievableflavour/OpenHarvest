using UnityEngine;

public class RandomSpeedObjectSpawner : MonoBehaviour
{
    public GameObject prefab;
    float Timer = 0;
    void Update()
    {
        Timer -= Time.deltaTime;
        if (Timer <= 0f)
        {
            var clone = Instantiate(prefab, transform.position, Quaternion.identity);
            clone.GetComponent<Rigidbody>().isKinematic = false;
            clone.GetComponent<Rigidbody>().useGravity = true;
            clone.transform.SetParent(this.transform);
            Destroy(clone, 3.0f);
            Timer = Random.Range(0.4f, 1.0f);
        }
    }
}
