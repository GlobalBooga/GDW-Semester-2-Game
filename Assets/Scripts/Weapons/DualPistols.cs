using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualPistols : Weapon
{
    public Gun rightPistol;
    public Gun leftPistol;
    private bool right = true;


    public override void Use()
    {
        if (!readyToUse) return;
        //readyToUse = false;

        if (rightPistol && right)
        {
            right = false;
            rightPistol.Use();
        }
        else if (leftPistol && !right)
        {
            right = true;
            leftPistol.Use();
        }
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
