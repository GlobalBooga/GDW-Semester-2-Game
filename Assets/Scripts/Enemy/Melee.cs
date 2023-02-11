using System.Collections;
using System.Collections.Generic;
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
        //Debug.Log("slash!");

        // play animation

        // // //

        Invoke(nameof(ResetAttack), attackCooldown);
        
        // if we hit
        if (PlayerDistance <= attackReach)
        {
            Invoke(nameof(ApplyDamage), applyDmgDelay);
        }
    }

    private void ApplyDamage()
    {
        HPComponent hp;
        if (playerLoc.gameObject.TryGetComponent(out hp))
        {
            hp.Reduce(damage);
        }
    }
}
