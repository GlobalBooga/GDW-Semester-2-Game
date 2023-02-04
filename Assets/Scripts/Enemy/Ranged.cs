using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : Enemy
{
    [Header("Ranged"), Space(5f)]
    public float damage;
    public float delayBetweenShots = 0.5f;
    public float shootStartDelay = 1f;
    public float bulletSpeed = 5f;
    public float bulletSpread = 1f;
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
        Debug.Log("attacck");
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
            float x = transform.right.x, y = transform.right.y;

            Vector3 bulletDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));

            // play animation

            // // //

            //Debug.Log($"deg: {spreadAngle}, rad: {rads}");
            Debug.DrawLine(transform.position, transform.position + bulletDir * 50f, Color.red, delayBetweenShots);
            //Debug.Log("pew!");

            if (bullet && bulletSpawn)
            {
                Bullet b = Instantiate(bullet, bulletSpawn).GetComponent<Bullet>();
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

        /*//Invoke(nameof(ResetAttack), attackCooldown);

            Realistic shooting approach - FOR SNIPER


        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 100f, whatTakesDamage);
        if (hit)
        {
            // calculate the time it will take to reach 'hit'


            HPComponent hp;
            if (hit.transform.gameObject.TryGetComponent(out hp))
            {
                hp.Reduce(damage);
            }
        }*/