using System.Collections;
using UnityEngine;

public class Melee : Enemy
{

    public AudioSource audioSource;
    public AudioClip atkSound;

    private MeleeScriptableObject mso;
    const string HIT_MELEE = "MeleeHit";

    internal override void Awake()
    {
        base.Awake();
        mso = (MeleeScriptableObject)eso;
        audioSource = GetComponent<AudioSource>();
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
        audioSource.Play();
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
