using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualPistols : Weapon
{
    public Gun rightPistol;
    public Gun leftPistol;
    public float cameraShakeIntensity;
    public float cameraShakeTime;
    private bool right = true;


    public override void Use()
    {
        if (!readyToUse) return;
        readyToUse = false;

        float cool = 0f;

        if (rightPistol && right)
        {
            right = false;
            rightPistol.cameraShakeIntensity = cameraShakeIntensity;
            rightPistol.cameraShakeTime = cameraShakeTime;
            rightPistol.Use();
            cool = rightPistol.cooldown;
        }
        else if (leftPistol && !right)
        {
            right = true;
            leftPistol.cameraShakeIntensity = cameraShakeIntensity;
            leftPistol.cameraShakeTime = cameraShakeTime;
            leftPistol.Use();
            cool = leftPistol.cooldown;
        }

        if (cool > 0) Invoke(nameof(ResetUse), cool);
        else ResetUse();
    }

    public override void UseAbility()
    {
        // play animation
        
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
