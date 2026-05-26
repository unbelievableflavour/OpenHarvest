using BNG;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnController : MonoBehaviour
{
    public Transform spawnLocation;
    public GameObject spawnEffect;
    public Transform spawnEffectLocation;
    public Text selectedItemLabel;
    public AudioClip leverSound;

    private bool canSpawn = false;
    private HarvestDataTypes.Item currentlySelectedItem = null;
    private List<string> alreadySpawnedItems = new List<string>();

    public void SetSelectedItem(HarvestDataTypes.Item item)
    {
        currentlySelectedItem = item;
        selectedItemLabel.text = item.name;
    }

    public void SetAlreadySpawnedError()
    {
        selectedItemLabel.text = "You already spawned this item. Reenter the farm to spawn again.";
    }

    public void SpawnItem(float leverPercentage)
    {
        if(leverPercentage > 45 && leverPercentage < 55)
        {
            canSpawn = true;
            return;
        }

        if((leverPercentage < 99 && leverPercentage > 1) || !canSpawn)
        {
            return;
        }

        if (currentlySelectedItem == null)
        {
            return;
        }

        if(alreadySpawnedItems.Contains(currentlySelectedItem.itemId)){
            SetAlreadySpawnedError();
            return;
        }

        VRUtils.Instance.PlaySpatialClipAt(leverSound, transform.position, 1f, 1f);
        Definitions.InstantiateItemNew(currentlySelectedItem.prefab, spawnLocation);
        alreadySpawnedItems.Add(currentlySelectedItem.itemId);

        if (spawnEffect)
        {
            var particle = Instantiate(spawnEffect, spawnEffectLocation.transform.position, spawnEffectLocation.transform.rotation);
            Object.Destroy(particle, 2.0f);
        }
    }
}
