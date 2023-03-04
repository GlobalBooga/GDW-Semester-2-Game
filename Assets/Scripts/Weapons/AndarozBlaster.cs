using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndarozBlaster : Gun
{
    [Header("Weapon Ability"), Space(5f)]
    public float newCooldown = 0.01f;
    public float abilityShots;
    public float delayBeforeAbility = 1f;
    private float temp;
    public ParticleSystem chargeParticles;

    internal override void Awake()
    {
        base.Awake();
    }

    internal override void OnValidate()
    {
        base.OnValidate();
    }

    internal override void ResetUse()
    {
        base.ResetUse();
    }

    public override void Use()
    {
        base.Use();
    }

    public override void UseAbility()
    {
        if (!canUseAbility) return;
        canUseAbility = false;
        readyToUse = false;

        base.UseAbility();

        temp = cooldown;   
        cooldown = newCooldown;

        if (chargeParticles) StartCoroutine(nameof(ChargeAndFireAbility));

    }

    private IEnumerator ChargeAndFireAbility()
    {
        if (chargeParticles)
        {
            chargeParticles.Play();
            while (chargeParticles.isPlaying) yield return null;
        }

        if (delayBeforeAbility > 0) yield return new WaitForSeconds(delayBeforeAbility);
        
        for (int i = 0; i < abilityShots; i++)
        {
            Fire();
            yield return new WaitForSeconds(newCooldown);
        }

        StartCoroutine(nameof(CooldownAbility));
    }

    public override void EndAbility()
    {
        cooldown = temp;
        readyToUse = true;
    }

    public override void Drop(Vector2 forwards)
    {
        base.Drop(forwards);
    }

    public override void Pickup(Transform parentTo, Weapon weapon)
    {
        base.Pickup(parentTo, weapon);
    }
}
