using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DualPistols : Weapon
{
    public DualPistols_Single rightPistol;
    public DualPistols_Single leftPistol;
    //public Image holdingSprite;
    public float cameraShakeIntensity;
    public float cameraShakeTime;
    private bool right = true;
    public bool alertEnemies = true;

    [Header("Weapon Ability"), Space(5f)]
    public float newCooldown = 0.08f;
    public float abilityShots;


    public override void Use()
    {
        if (!readyToUse) return;
        readyToUse = false;

        if (rightPistol && right)
        {
            right = false;
            UpdateGunProperties(rightPistol);
            rightPistol.Use();
        }
        else if (leftPistol && !right)
        {
            right = true;
            UpdateGunProperties(leftPistol);
            leftPistol.Use();
        }

        if (cooldown > 0) Invoke(nameof(ResetUse), cooldown);
        else ResetUse();
    }

    public override void Drop(Vector2 forwards)
    {
        base.Drop(forwards);
        rightPistol.Drop();
        leftPistol.Drop();
    }

    public override void Pickup(Transform parentTo, Weapon weapon)
    {
        base.Pickup(parentTo, weapon);
        rightPistol.Pickup();
        leftPistol.Pickup();
    }

    public override void UseAbility()
    {
        if (!canUseAbility) return;
        canUseAbility = false;
        readyToUse = false;

        LevelManager.instance.DisablePlayerRotation();

        base.UseAbility();

        UpdateGunProperties(rightPistol);
        UpdateGunProperties(leftPistol);

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

    private void UpdateGunProperties(DualPistols_Single gun)
    {
        gun.damage = damage;
        gun.cameraShakeIntensity = cameraShakeIntensity;
        gun.cameraShakeTime = cameraShakeTime;
        gun.cooldown = cooldown;
        gun.newCooldown = newCooldown;
        gun.abilityShots = abilityShots;
        gun.alertsEnemies = alertEnemies;
    }

}
