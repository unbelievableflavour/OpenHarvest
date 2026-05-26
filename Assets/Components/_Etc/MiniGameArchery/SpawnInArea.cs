using System.Collections.Generic;
using UnityEngine;

public class SpawnInArea : MonoBehaviour
{
    public Collider collider;
    public GameObject target;
    public MiniGameArchery waveController;
    public float numberOfSecondsBeforeFirstSpawn = 3f;

    private List<GameObject> enemies = new List<GameObject>();

    public void SpawnEnemy()
    {
        SpawnEnemyInCube();
    }

    void SpawnEnemyInCube()
    {
        Vector3 spawnPosition = RandomPointInBounds(collider.bounds);
        
        var enemy = Instantiate(target, spawnPosition, Quaternion.identity) as GameObject;
        enemy.GetComponent<MiniGameTarget>().SetParent(waveController);
        enemies.Add(enemy);
    }

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public void RemoveActiveEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            GameObject.Destroy(enemy);
        }
    }
}