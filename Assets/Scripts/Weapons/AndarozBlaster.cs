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

    public override int GetID()
    {
        return 4;
    }
    public override void UseAbility()
    {
        if (!canUseAbility) return;
        canUseAbility = false;
        readyToUse = false;
        usingAbility = true;

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
            if (i == abilityShots - 1) usingAbility = false;
            Fire();
            yield return new WaitForSeconds(newCooldown);
        }

        StartCoroutine(nameof(CooldownAbility));
    }

    public override void EndAbility()
    {
        base.EndAbility();

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
