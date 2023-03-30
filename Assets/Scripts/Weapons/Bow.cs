using System.Collections;
using System.Linq;
using UnityEngine;

public class Bow : Gun
{
    [Header("Bow Properties"), Space(5f)]
    public float lockOnDistance = 20f;
    public float lockOnAngle = 60f;
    public float homingForce = 10f;
    public float arrowLifetime = 0.5f;
    public LayerMask whatIsEnemy;
    public LayerMask whatBlocksSight;

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

        if (bullet && bulletSpawn)
        {
            HomingArrow a = Instantiate(bullet, bulletSpawn).GetComponent<HomingArrow>();

            audioSource.Play();

            yield return new WaitForSeconds(0.4f);

            Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, lockOnDistance, whatIsEnemy);

            if (col.Length > 0)
            {
                col = col.OrderBy((d) => (d.transform.position - transform.position).sqrMagnitude).ToArray();
            }

            for (int n = 0; n < 2; n++)
            {
                if (target) break;

                for (int i = 0; i < col.Length; i++)
                {
                    // check if we are facing this enemy
                    if (Vector3.Angle(transform.up, col[i].transform.position - transform.position) <= lockOnAngle * 0.5f)
                    {
                        // operation 1: lockon to a visible target
                        if (n == 0)
                        {
                            // is anything blocking the way
                            RaycastHit2D hit = Physics2D.Raycast(transform.position, (col[i].transform.position - transform.position).normalized, lockOnDistance, whatBlocksSight);
                            if (hit.transform.gameObject.layer == StaticHelpers.EnemyLayer ||
                                hit.transform.gameObject.layer == StaticHelpers.EnemyMissileLayer ||
                                hit.transform.gameObject.layer == StaticHelpers.TrainWeaponLayer)
                            {
                                target = col[i].transform;
                                break;
                            }
                        }
                        // operation 2 : lockon to target behind a wall
                        else if (n == 1)
                        {
                            target = col[i].transform;
                            break;
                        }
                    }
                }
            }

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

        //lockon to a target
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, lockOnDistance, whatIsEnemy);

        if (col.Length > 0)
        {
            col = col.OrderBy((d) => (d.transform.position - transform.position).sqrMagnitude).ToArray();
        }

        for (int o = 0; o < 2; o++)
        {
            // for every target
            for (int t = 0, i = 0; t < targets.Length; t++)
            {
                // enemy selector
                for (; i < col.Length; i++)
                {
                    // check if we are facing this enemy
                    if (Vector3.Angle(transform.up, col[i].transform.position - transform.position) <= lockOnAngle * 0.5f)
                    {
                        if (targets.Length > col.Length || o == 1)
                        {
                            targets[t] = col[i].transform;
                            i++;
                            break;
                        }
                        else
                        {
                            // is anything blocking the way
                            RaycastHit2D hit = Physics2D.Raycast(transform.position, (col[i].transform.position - transform.position).normalized, lockOnDistance, whatBlocksSight);
                            if (hit.transform.gameObject.layer == StaticHelpers.EnemyLayer ||
                                hit.transform.gameObject.layer == StaticHelpers.EnemyMissileLayer ||
                                hit.transform.gameObject.layer == StaticHelpers.TrainWeaponLayer)
                            {
                                targets[t] = col[i].transform;
                                i++;
                                break;
                            }
                        }
                    }
                }
                if (i >= col.Length) i = 0;
            }

            if (!targets.Last()) continue;

            break;
        }

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
