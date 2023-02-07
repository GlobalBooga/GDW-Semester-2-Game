using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : Enemy
{
    [Header("Ranged"), Space(5f)]
    public float damage;
    public float delayBetweenShots = 0.5f;
    public float shootStartDelay = 0.3f;
    public float bulletSpeed = 15f;
    public float bulletSpread = 10f;
    public bool bouncyBullets = false;
    public int bounces = 3;
    public GameObject bullet;
    public Transform bulletSpawn;

    internal override void Awake()
    {
        base.Awake();
    }

    internal override void Start()
    {
        base.Start();
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
        //Debug.Log("attacck");
        StartCoroutine(nameof(Shoot));

    }

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(shootStartDelay);

        while (CanSeePlayer())
        {
            // calculate spread
            float spreadAngle = Random.Range(-bulletSpread, bulletSpread);
            float rads = Mathf.Deg2Rad * ((spreadAngle > 0) ? spreadAngle : (360f + spreadAngle));
            float x = body.right.x, y = body.right.y;

            Vector3 bulletDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));

            // play animation

            // // //

            if (showDebugStuff) Debug.DrawLine(body.position, body.position + bulletDir * 50f, Color.red, delayBetweenShots);

            if (bullet && bulletSpawn)
            {
                Bullet b = Instantiate(bullet, bulletSpawn).GetComponent<Bullet>();
                b.transform.Rotate(0f,0f, Vector2.SignedAngle(body.right, bulletDir));
                b.gameObject.layer = LayerMask.NameToLayer(ENEMY_PROJECTILE_LAYER);
                b.Fly(bulletDir, bulletSpeed, damage);

                //play muzzle effect
            }

            yield return new WaitForSeconds(delayBetweenShots);
        }
    }

    internal override void OnLostSightOfPlayer()
    {
        base.OnLostSightOfPlayer();
        StopCoroutine(nameof(Shoot));
        base.ResetAttack();
    }
}
