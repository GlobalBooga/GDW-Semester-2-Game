using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Melee : Enemy
{
    [Header("Melee"), Space(5f)]
    public float damage;
    public float attackCooldown = 1f;
    public float applyDmgDelay = 0.5f;
    private float attackReach;

    internal override void Awake()
    {
        base.Awake();
    }

    internal override void Start()
    {
        base.Start();
        attackReach = maxAttackDistance;
    }

    internal override void Update() 
    {
        base.Update();
    }

    internal override void OnValidate()
    {
        base.OnValidate();
    }

    internal override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    internal override void Attack()
    {
        base.Attack();

        StartCoroutine(nameof(Slash));
    }

    private IEnumerator Slash()
    {
        // play animation

        yield return new WaitForSeconds(applyDmgDelay);

        // if we hit
        if (PlayerDistance <= attackReach)
        {
            //Invoke(nameof(ApplyDamage), applyDmgDelay);
            StaticHelpers.ApplyDamage(playerLoc.gameObject, damage);
        }


        yield return new WaitForSeconds(attackCooldown);
        ResetAttack();
    }

    
}
