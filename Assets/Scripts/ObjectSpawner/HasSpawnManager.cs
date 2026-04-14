using UnityEngine;

public class HasSpawnManager : MonoBehaviour
{
    private SpawnManager originatedSpawnManager;

    public void resetSpawnManagerIfLastGrabbed()
    {
        if (!originatedSpawnManager)
        {
            return;
        }
        originatedSpawnManager.resetGrowthIfLastChild();
        originatedSpawnManager = null;
    }

    public void resetGrowth()
    {
        if (!originatedSpawnManager)
        {
            return;
        }
        originatedSpawnManager.resetGrowth();
        originatedSpawnManager = null;
    }

    public void SetOriginatedSpawnManager(SpawnManager spawnManager)
    {
        originatedSpawnManager = spawnManager;
    }
}
