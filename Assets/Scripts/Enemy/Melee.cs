using System.Collections;
using UnityEngine;

public class Melee : Enemy
{
    
    private MeleeScriptableObject mso;
    const string HIT_MELEE = "MeleeHit";

    internal override void Awake()
    {
        base.Awake();
        mso = (MeleeScriptableObject)eso;
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
        if (PlayerDistance <= eso.defaultMaxAttackDistance)
        {
            //Invoke(nameof(ApplyDamage), applyDmgDelay);
            StaticHelpers.ApplyDamage(playerLoc.gameObject, mso.damage);
        }


        yield return new WaitForSeconds(mso.attackCooldown);
        ResetAttack();
    }

    internal override void Hit()
    {
        if (animator.isActiveAndEnabled)
        {
            animator.Play(HIT_MELEE);
        }
    }

}
