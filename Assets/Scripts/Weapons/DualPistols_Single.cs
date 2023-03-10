using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DualPistols_Single : Gun
{
    [HideInInspector] public float newCooldown;
    [HideInInspector] public float abilityShots;
    private float temp;

    public override void EndAbility()
    {
        base.EndAbility();
        readyToUse = true;
    }

    public new void Drop()
    {
        if (sr) sr.enabled = true; 
    }

    public void Pickup()
    {
        if (sr) sr.enabled = false;
    }
}