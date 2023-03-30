using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DualPistols : Weapon
{
    public DualPistols_Single rightPistol;
    public DualPistols_Single leftPistol;
    public float cameraShakeIntensity;
    public float cameraShakeTime;
    public float bulletSpread;
    public float bulletSpeed;
    private bool right = true;
    public bool alertEnemies = true;


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
        base.UseAbility();
        UpdateGunProperties(rightPistol);
        UpdateGunProperties(leftPistol);
    }

    public virtual void UpdateGunProperties(DualPistols_Single gun)
    {
        gun.damage = damage;
        gun.cameraShakeIntensity = cameraShakeIntensity;
        gun.cameraShakeTime = cameraShakeTime;
        gun.cooldown = cooldown;
        gun.alertsEnemies = alertEnemies;
        gun.bulletSpeed = bulletSpeed;
        gun.bulletSpread = bulletSpread;
    }

}
