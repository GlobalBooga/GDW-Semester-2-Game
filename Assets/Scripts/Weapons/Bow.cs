using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bow : Gun
{
    [Header("Bow Properties"), Space(5f)]
    public float lockOnDistance = 20f;
    public float lockOnAngle = 60f;
    public float homingForce = 10f;
    public float arrowLifetime = 0.5f;
    public LayerMask whatIsEnemy;

    [Header("Ability"), Space(5f)]
    public Transform deg15Spawn;
    public Transform deg30Spawn;
    public Transform neg15Spawn;
    public Transform neg30Spawn;


    public override int GetID()
    {
        return 3;
    }

    public override void Use()
    {
        if (!readyToUse) return;
        readyToUse = false;

        StartCoroutine(ShootArrow());
    }
    
    IEnumerator ShootArrow()
    {
        // custom bow use behaviour since it is not a gun

        Transform target = null;

        //lockon to a target
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, lockOnDistance, whatIsEnemy);
        foreach (var item in col)
        {
            // check if we are facing this enemy
            if (Vector3.Angle(transform.up, item.transform.position - transform.position) <= lockOnAngle * 0.5f)
            {
                target = item.transform;
            }
        }

        if (bullet && bulletSpawn)
        {
            HomingArrow a = Instantiate(bullet, bulletSpawn).GetComponent<HomingArrow>();

            yield return new WaitForSeconds(0.4f);
            
            if (target) a.Fly(target, transform.parent.up, homingForce, bulletSpeed, damage);
            else a.Fly(transform.up, bulletSpeed, damage);
            Destroy(a.gameObject, arrowLifetime);
        }

        if (cooldown > 0) Invoke(nameof(ResetUse), cooldown);
        else ResetUse();
    }


    public override void UseAbility()
    {
        if (!canUseAbility) return;
        canUseAbility = false;
        readyToUse = false;
        usingAbility = true;

        base.UseAbility();


        StartCoroutine(Ability());
    }

    IEnumerator Ability()
    {
        Transform[] targets = new Transform[5];

        //lockon to a target
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, lockOnDistance, whatIsEnemy);
        for (int n = 0, i = 0; n < targets.Length; n++)
        {

            for (; i < col.Length; i++)
            {
                // check if we are facing this enemy
                if (Vector3.Angle(transform.up, col[i].transform.position - transform.position) <= lockOnAngle * 0.5f)
                {
                    targets[n] = col[i].transform;
                    i++;
                    break;
                }
            }
            if (i >= col.Length) i = 0;
        }

        HomingArrow[] arrows = new HomingArrow[5];

        arrows[0] = Instantiate(bullet, bulletSpawn).GetComponent<HomingArrow>();
        arrows[0].GetComponent<BoxCollider2D>().enabled = false;

        yield return new WaitForSeconds(0.4f);
        arrows[1] = Instantiate(bullet, deg15Spawn).GetComponent<HomingArrow>();
        arrows[2] = Instantiate(bullet, neg15Spawn).GetComponent<HomingArrow>();
        arrows[1].GetComponent<BoxCollider2D>().enabled = false;
        arrows[2].GetComponent<BoxCollider2D>().enabled = false;

        yield return new WaitForSeconds(0.4f);
        arrows[3] = Instantiate(bullet, deg30Spawn).GetComponent<HomingArrow>();
        arrows[4] = Instantiate(bullet, neg30Spawn).GetComponent<HomingArrow>();
        arrows[3].GetComponent<BoxCollider2D>().enabled = false;
        arrows[4].GetComponent<BoxCollider2D>().enabled = false;

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < arrows.Length; i++)
        {
            if (targets[i]) arrows[i].Fly(targets[i], arrows[i].transform.up, homingForce, bulletSpeed, damage);
            else arrows[i].Fly(arrows[i].transform.up, bulletSpeed, damage);
            Destroy(arrows[i].gameObject, arrowLifetime);
            arrows[i].GetComponent<BoxCollider2D>().enabled = true;
        }

        StartCoroutine(nameof(CooldownAbility));
    }


    public override void EndAbility()
    {
        base.EndAbility();
        readyToUse = true;
        usingAbility = false;
    }

    
}
