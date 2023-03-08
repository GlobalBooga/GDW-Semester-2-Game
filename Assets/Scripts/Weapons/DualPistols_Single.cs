using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DualPistols_Single : Gun
{
    [HideInInspector] public float newCooldown;
    [HideInInspector] public float abilityShots;
    private float temp;

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