using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Melee : Enemy
{
    
    private MeleeScriptableObject mso;

    internal override void Awake()
    {
        base.Awake();
        mso = (MeleeScriptableObject)eso;
    }

    internal override void Start()
    {
        base.Start();
        //attackReach = eso.maxAttackDistance;
    }

    internal override void Update() 
    {
        base.Update();
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

        yield return new WaitForSeconds(mso.applyDmgDelay);

        // if we hit
        if (PlayerDistance <= eso.maxAttackDistance)
        {
            //Invoke(nameof(ApplyDamage), applyDmgDelay);
            StaticHelpers.ApplyDamage(playerLoc.gameObject, mso.damage);
        }


        yield return new WaitForSeconds(mso.attackCooldown);
        ResetAttack();
    }

    
}
