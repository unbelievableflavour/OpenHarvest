using BNG;
using System.Collections;
using UnityEngine;

public class BreakableObjectController : MonoBehaviour
{
    [Header("Health")]
    public int health = 3;
    public UnityEngine.UI.Slider healthBar;

    [Header("Tool")]
    public string toolUsedForBreaking = "Hammer";
    
    [Header("Sounds & Effects")]
    public GameObject hitEffect;
    public AudioClip[] hitSounds;
    public AudioClip[] secondHitSounds;
    public AudioClip[] breakSounds;

    [Header("Etc")]
    public bool useOnCollisionEnter = true;
    public bool dontDestroyOnBreak = false;

    public bool useCooldownCollisionZone = false;

    public UnityEngine.Events.UnityEvent afterBreakFunctions;

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

        if (healthBar)
        {
            healthBar.value = (1f / backupHealth * health);
        }

        inCooldown = true;

        if (!useCooldownCollisionZone && !isBroken())
        {
            StartCoroutine(Cooldown());
        }

        if (isBroken() && !alreadyBroken)
        {
            PlayRandomBreakSounds();
            alreadyBroken = true;
            afterBreakFunctions.Invoke();
            if (dontDestroyOnBreak)
            {
                return;
            }

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

    void OnCollisionEnter(Collision collision)
    {
        if (!useOnCollisionEnter || inCooldown)
        {
            return;
        }

        var collider_tag = collision.gameObject.tag;

        if (collider_tag != toolUsedForBreaking)
        {
            return;
        }

        var damage = collision.gameObject.GetComponent<Damage>();
        hitStone(damage ? damage.damage : 1);
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
            VRUtils.Instance.PlaySpatialClipAt(hitSounds[Random.Range(0, hitSounds.Length)], transform.position, 1f, 1f);
        }

        if (secondHitSounds.Length != 0)
        {
            VRUtils.Instance.PlaySpatialClipAt(secondHitSounds[Random.Range(0, secondHitSounds.Length)], transform.position, 1f, 1f);
        }
    }

    public void PlayRandomBreakSounds()
    {
        if (breakSounds.Length != 0)
        {
            VRUtils.Instance.PlaySpatialClipAt(breakSounds[Random.Range(0, breakSounds.Length)], transform.position, 1f, 1f);
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

        if (healthBar) 
        {
            healthBar.value = 1;
        }
    }

    public bool isAlreadyBroken()
    {
        return alreadyBroken;
    }
}
