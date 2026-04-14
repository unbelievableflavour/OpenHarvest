using UnityEngine;

public class SpawnerSelector : MonoBehaviour
{
    private HarvestDataTypes.Item item;
    private SpawnController spawnController;

    public void SetSpawnController(SpawnController newSpawnController)
    {
        spawnController = newSpawnController;
    }

    public void SetItem(HarvestDataTypes.Item item)
    {
        this.item = item;
    }

    public void SetSpawnerItemToCurrent()
    {
        spawnController.SetSelectedItem(item);
    }
}
