using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    [Header("Properties"), Space(5f)]
    public float damage;
    public float cooldown;
    [HideInInspector] public bool readyToUse = true;
    public bool isAutoUse;
    public ParticleSystem pickupIndicator;
    public Sprite holdingSprite;
    public Sprite iconSprite;

    public float abilityCooldown = 6f;
    internal bool canUseAbility = true;
    internal bool usingAbility;

    private Image weaponAbilityCooldown;
    private CircleCollider2D cc;
    private Rigidbody2D rb;
    public SpriteRenderer sr;

    private const float startDelay = 0.25f;

    public Action OnAbilityEnded;

    public virtual int GetID()
    {
        return 0;
    }


    internal virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        weaponAbilityCooldown = Hud.instance.abilityCooldown;
        
    }

    internal virtual void OnValidate()
    {
        if (isAutoUse && cooldown < 0.01f)
        {
            cooldown = 0.1f;
        }
    }

    public virtual void Use()
    {

    }

    public virtual void UseAbility()
    {
        if (weaponAbilityCooldown) weaponAbilityCooldown.fillAmount = 0f;
    }

    public virtual void EndAbility()
    {
        
    }

    internal virtual void ResetUse()
    {
        if (!usingAbility) readyToUse = true;
    }

    public void Drop()
    {
        Drop(Vector2.zero);
    }

    public virtual void Drop(Vector2 forwards)
    {
        rb.simulated = true;
        transform.parent = null;
        rb.velocity = Vector2.zero;
        rb.AddForce(forwards * 2.5f, ForceMode2D.Impulse);
        Invoke(nameof(SetPickupable), startDelay);
        
        if (sr) sr.enabled = true;
    }

    public virtual void Pickup(Transform parentTo, Weapon weapon)
    {
        weapon = this;
        transform.parent = parentTo;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        SetHeld();

        if (sr) sr.enabled = false;
        if (holdingSprite) LevelManager.instance.SetPlayerSprite(holdingSprite);
        if (iconSprite) Hud.instance.SetWeaponIconImage(iconSprite);
        else LevelManager.instance.SetPlayerSprite(null);
    }

    private void SetPickupable()
    {
        cc.enabled = true;
        pickupIndicator.gameObject.SetActive(true);
    }

    public virtual void SetHeld()
    {
        rb.simulated = cc.enabled = false;
        pickupIndicator.gameObject.SetActive(false);
    }

    public IEnumerator CooldownAbility()
    {
        EndAbility();

        // cooldown
        float time = 0f;
        if (weaponAbilityCooldown)
        {
            while (time < abilityCooldown)
            {
                if (time > 0) weaponAbilityCooldown.fillAmount = time / abilityCooldown;
                time += Time.deltaTime;
                yield return null;
            }
        }
        else yield return new WaitForSeconds(abilityCooldown);


        // end of cooldown
        canUseAbility = true;
    }
}