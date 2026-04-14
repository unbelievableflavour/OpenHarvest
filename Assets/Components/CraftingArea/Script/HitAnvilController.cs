using BNG;
using System.Collections;
using UnityEngine;

public class HitAnvilController : MonoBehaviour
{
    [Header("Tool")]
    public string toolUsedForBreaking = "Hammer";
    
    [Header("Sounds & Effects")]
    public GameObject hitEffect;
    public AudioClip[] hitSounds;

    [Header("Etc")]
    public bool useCooldownCollisionZone = false;

    public UnityEngine.Events.UnityEvent afterHitFunctions;

    private float cooldown = 0.5f;
    private bool inCooldown = false;

    public void hitStone()
    {
        inCooldown = true;

        if (!useCooldownCollisionZone)
        {
            StartCoroutine(Cooldown());
        }

        afterHitFunctions.Invoke();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (inCooldown)
        {
            return;
        }

        var collider_tag = collision.gameObject.tag;

        if (collider_tag != toolUsedForBreaking)
        {
            return;
        }

        hitStone();
        PlayEffect(collision);
        PlayRandomHitSounds();
    }


    public void PlayEffect(Collision collision)
    {
        if (!hitEffect)
        {
            return;
        }

        ContactPoint contact = collision.contacts[0];
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 position = contact.point;
        var particle = Instantiate(hitEffect, position, rotation);
        Object.Destroy(particle, 2.0f);
    }

    public void PlayRandomHitSounds()
    {
        if (hitSounds.Length != 0)
        {
            VRUtils.Instance.PlaySpatialClipAt(hitSounds[Random.Range(0, hitSounds.Length)], transform.position, 1f);
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        unsetCooldown();
    }

    public void SetCooldownManually()
    {
        inCooldown = true;
    }

    public void unsetCooldown()
    {
        inCooldown = false;
    }

    public void Reset()
    {
        inCooldown = false;
    }
}
