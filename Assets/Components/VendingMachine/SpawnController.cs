using BNG;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Definitions;

public class SpawnController : MonoBehaviour
{
    public Transform spawnLocation;
    public GameObject spawnEffect;
    public Transform spawnEffectLocation;
    public Text selectedItemLabel;
    public AudioClip leverSound;

    private bool canSpawn = false;
    private string currentlySelectedItem = "";
    private List<string> alreadySpawnedItems = new List<string>();

    public void SetSelectedItem(string itemId)
    {
        currentlySelectedItem = itemId;
        selectedItemLabel.text = GetItemInformation(itemId).name;
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

        Item itemInfo = GetItemInformation(currentlySelectedItem);
        if (itemInfo == null)
        {
            return;
        }

        if(alreadySpawnedItems.Contains(currentlySelectedItem)){
            SetAlreadySpawnedError();
            return;
        }

        VRUtils.Instance.PlaySpatialClipAt(leverSound, transform.position, 1f, 1f);
        InstantiateItem(itemInfo.prefabFileName, spawnLocation);
        alreadySpawnedItems.Add(currentlySelectedItem);

        if (spawnEffect)
        {
            var particle = Instantiate(spawnEffect, spawnEffectLocation.transform.position, spawnEffectLocation.transform.rotation);
            Object.Destroy(particle, 2.0f);
        }
    }
}
