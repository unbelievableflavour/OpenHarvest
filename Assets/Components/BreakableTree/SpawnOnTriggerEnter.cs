using BNG;
using UnityEngine;

public class SpawnOnTriggerEnter : MonoBehaviour
{
    public ObjectSpawner objectSpawner;
    public GameObject fallingTree;
    public GameObject particleEffect;
    public Transform spawnLocations;
    public AudioClip[] breakSounds;

    bool isAlreadyTriggered = false;

    private void Awake()
    {
        Invoke("DoTheThing", 5);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.name == "RemoteGrabber")
        {
            return;
        }

        DoTheThing();
    }

    public void DoTheThing()
    {
        if (isAlreadyTriggered)
        {
            return;
        }
        isAlreadyTriggered = true;

        foreach (Transform location in spawnLocations)
        {
            GameObject particle = Instantiate(particleEffect, new Vector3(
                location.position.x,
                location.position.y,
                location.position.z
            ),
            location.transform.rotation);
            Destroy(particle, 5.0f);
        }

        objectSpawner.SpawnFruit();
        objectSpawner.gameObject.SetActive(false);
        PlayRandomBreakSounds();
    }

    public void Reset()
    {
        isAlreadyTriggered = false;
        fallingTree.GetComponent<Rigidbody>().velocity = Vector3.zero;
        fallingTree.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        fallingTree.transform.localPosition = new Vector3(0,0,0);
        fallingTree.transform.localRotation = new Quaternion(0,0,0,0);   
    }

    public void PlayRandomBreakSounds()
    {
        if (breakSounds.Length != 0)
        {
            VRUtils.Instance.PlaySpatialClipAt(breakSounds[Random.Range(0, breakSounds.Length)], transform.position, 1f, 1f);
        }
    }
}
