using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdanasDualies : DualPistols
{
    [Header("Weapon Ability"), Space(5f)]
    public float newCooldown = 0.08f;
    public float abilityShots;

    public override int GetID()
    {
        return 1;
    }

    public override void UseAbility()
    {
        if (!canUseAbility) return;
        canUseAbility = false;
        readyToUse = false;

        LevelManager.instance.DisablePlayerRotation();

        base.UseAbility();
        
        StartCoroutine(nameof(FireAbility));
    }

    private IEnumerator FireAbility()
    {
        for (int i = 0; i < abilityShots; i++)
        {
            rightPistol.Fire();
            leftPistol.Fire();
            yield return new WaitForSeconds(newCooldown);
        }

        StartCoroutine(nameof(CooldownAbility));
    }

    public override void EndAbility()
    {
        base.EndAbility();
        LevelManager.instance.EnablePlayerRotation();
        readyToUse = true;
    }

    public override void UpdateGunProperties(DualPistols_Single gun)
    {
        base.UpdateGunProperties(gun);
        gun.newCooldown = newCooldown;
        gun.abilityShots = abilityShots;
    }
}
