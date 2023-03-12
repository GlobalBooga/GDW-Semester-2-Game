using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : Enemy
{
    
    public Transform bulletSpawn;
    private RangedScriptableObject rso;
    const string HIT_RANGED = "RangedHit";

    internal override void Awake()
    {
        base.Awake();
        rso = (RangedScriptableObject)eso;
    }
    
    internal override void Attack()
    {
        base.Attack();
        //Debug.Log("attacck");
        StartCoroutine(nameof(Shoot));
    }

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(rso.shootStartDelay);

        // alert interval
        float timeInterval = 1f;
        float time = Time.time - timeInterval;

        while (CanSeePlayer())
        {
            // alert everyone
            if (Time.time - time >= timeInterval)
            {
                time = Time.time;
                LevelManager.instance.AlertAllEnemiesInCurrentScene(transform.position);
            }

            // calculate spread
            float spreadAngle = Random.Range(-rso.bulletSpread, rso.bulletSpread);
            float rads = Mathf.Deg2Rad * ((spreadAngle > 0) ? spreadAngle : (360f + spreadAngle));
            float x = body.up.x, y = body.up.y;

            Vector3 bulletDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));

            // muzzleFlash
            if (rso.muzzleFlash) rso.muzzleFlash.SetActive(true);

            if (showDebugStuff) Debug.DrawLine(body.position, body.position + bulletDir * 50f, Color.red, rso.delayBetweenShots);

            if (rso.bullet && bulletSpawn)
            {
                Bullet b = Instantiate(rso.bullet, bulletSpawn).GetComponent<Bullet>();
                b.transform.Rotate(0f,0f, Vector2.SignedAngle(body.up, bulletDir));
                b.gameObject.layer = StaticHelpers.EnemyProjectileLayer;
                b.Fly(bulletDir, rso.bulletSpeed, rso.damage);

                //play muzzle effect
            }

            yield return new WaitForSeconds(rso.delayBetweenShots);
        }
    }

    internal override void OnLostSightOfPlayer()
    {
        base.OnLostSightOfPlayer();
        StopCoroutine(nameof(Shoot));
        base.ResetAttack();
    }

    internal override void Hit()
    {
        if (animator.isActiveAndEnabled)
        {
            animator.Play(HIT_RANGED);
        }
    }
}
