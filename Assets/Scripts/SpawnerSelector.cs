using UnityEngine;

public class SpawnerSelector : MonoBehaviour
{
    private string itemId;
    private SpawnController spawnController;

    public void SetSpawnController(SpawnController newSpawnController)
    {
        spawnController = newSpawnController;
    }

    public void SetItem(string itemId)
    {
        this.itemId = itemId;
    }

    public void SetSpawnerItemToCurrent()
    {
        spawnController.SetSelectedItem(itemId);
    }
}
