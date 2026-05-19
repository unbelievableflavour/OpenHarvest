using BNG;
using System.Collections;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    [Header("Health")]
    public int health = 3;

    [Header("Sounds & Effects")]
    public GameObject hitEffect;
    public AudioClip[] hitSounds;

    private string mouthTag = "Face";
    private float cooldown = 0.5f;
    private bool inCooldown = false;
    private bool alreadyBroken = false;
    private int backupHealth;

    void Start()
    {
        backupHealth = health;
    }

    public void hitStone(int damage = 1)
    {
        health = health - damage;

        inCooldown = true;

        if (!isBroken())
        {
            StartCoroutine(Cooldown());
        }

        if (isBroken() && !alreadyBroken)
        {
            alreadyBroken = true;

            var itemStack = GetComponent<ItemStack>();

            if (itemStack && itemStack.GetStackSize() > 1)
            {
                itemStack.DecreaseStack(1);
                Reset();
                return;
            }
            Destroy(gameObject, 0.5f);
            return;
        }
    }

    public bool isBroken()
    {
        return health <= 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (inCooldown)
        {
            return;
        }

        var collider_tag = other.gameObject.tag;

        if (collider_tag != mouthTag)
        {
            return;
        }

        //Dont make it edible when its still snapped.
        if (GetComponent<Grabbable>() && transform.parent && transform.parent.GetComponent<SnapZone>())
        {
            return;
        }

        var damage = other.gameObject.GetComponent<Damage>();
        hitStone(damage ? damage.damage : 1);

        if (hitEffect)
        {
            var particle = Instantiate(hitEffect, other.transform.position, other.transform.rotation);
            Object.Destroy(particle, 2.0f);
        }

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
            VRUtils.Instance.PlaySpatialClipAt(hitSounds[Random.Range(0, hitSounds.Length)], transform.position, 1f, 1f);
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
        alreadyBroken = false;
        health = backupHealth;
    }

    public bool isAlreadyBroken()
    {
        return alreadyBroken;
    }
}
