using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Andaroz : Enemy
{
    public enum AndarozAttacks
    {
        Melee_Swipe,
        Melee_AOEGroundSlam,
        Ranged_MachineGun,
        Ranged_AOEMissileBarrage,
        Ranged_UltTwinLaser
    }

    [Header("Melee Attack 1 - Swipe"), Space(5f)]
    public float swipeDamage;
    public float swipeCooldown = 1f;
    public float swipeApplyDmgDelay = 0.5f;
    public float swipeReach;

    [Space(10f)]
    [Header("Melee Attack 2 - AOE Ground Slam"), Space(5f)]
    public float slamDamage;
    public float slamCooldown = 1f;
    public float slamApplyDmgDelay = 0.5f;
    public float slamReach;

    [Space(10f)]

    [Header("Ranged Attack 1 - Machine Gun"), Space(5f)]
    public float gunDamage;
    public float delayBetweenShots_gun = 0.5f;
    public float shootStartDelay_gun = 0.3f;
    public float bulletSpeed = 15f;
    public float bulletSpread = 10f;
    public int shots_bullets = 50;
    public GameObject bullet;
    public Transform bulletSpawn;

    [Space(10f)]

    [Header("Ranged Attack 2 - Missile Barrage"), Space(5f)]
    public float missileDamage;
    public float delayBetweenShots_missiles = 0.5f;
    public float shootStartDelay_missiles = 0.3f;
    public float missileSpeed = 7f;
    public float missileSpread = 10f;
    public int shots_missiles = 10;
    public GameObject missile;
    public List<Transform> missileSpawns;

    [Space(10f)]

    [Header("Ranged Attack 3 - Twin Laser"), Space(5f)]
    public float laserDamage;
    public float attackDuration;
    public float rotationSpeed;
    public float maxDist = 100f;
    public List<Transform> laserStart;
    public List<LineRenderer> lineRenderer;

    [Space(10f)]

    [Header("Attack Pattern"), Space(5f)]
    public List<string> orderedAttacks;

    private List<string> allAttacks = new() { nameof(PerformGunAttack), nameof(PerformLaserAttack), nameof(PerformMissileAttack), nameof(PerformSlamAttack), nameof(PerformSwipeAttack)};
    private Queue<string> attackPattern = new();


    internal override void Awake()
    {
        base.Awake();
    }

    internal override void Start()
    {
        base.Start();
        NewAttackOrder();
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
        NextAttack();

    }

    private IEnumerator PerformSwipeAttack()
    {
        Debug.Log("swipe");
        yield return new WaitForSeconds(3);
        NextAttack();
    }
    private IEnumerator PerformSlamAttack()
    {
        Debug.Log("slam");
        yield return new WaitForSeconds(3);
        NextAttack();
    }

    private IEnumerator PerformMissileAttack()
    {
        // homing missile attack

        Debug.Log("missile");
        yield return new WaitForSeconds(3);

        yield return new WaitForSeconds(shootStartDelay_missiles);
        int shots = 0;
        while (shots++ < shots_missiles)
        {
            // calculate spread
            float spreadAngle = Random.Range(-missileSpread, missileSpread);
            float rads = Mathf.Deg2Rad * ((spreadAngle > 0) ? spreadAngle : (360f + spreadAngle));
            float x = body.right.x, y = body.right.y;

            Vector3 missileDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));

            // play animation

            // // //

            if (showDebugStuff) Debug.DrawLine(body.position, body.position + missileDir * 50f, Color.red, delayBetweenShots_missiles);

            if (missile && missileSpawns.Count > 0)
            {
                Bullet b = Instantiate(bullet, missileSpawns[Random.Range(0, missileSpawns.Count - 1)]).GetComponent<Bullet>();
                b.transform.Rotate(0f, 0f, Vector2.SignedAngle(body.right, missileDir));
                b.gameObject.layer = StaticHelpers.EnemyProjectileLayer;
                b.Fly(missileDir, missileSpeed, missileDamage);

                //play muzzle effect
            }
            yield return new WaitForSeconds(delayBetweenShots_missiles);
        }
        NextAttack();
    }

    private IEnumerator PerformGunAttack()
    {
        // GUN ATTACK
        // in this attack, Andaroz chases the player at walking speed.
        // he is always facing the player

        Debug.Log("gun");
        yield return new WaitForSeconds(shootStartDelay_gun);
        int shots = 0;
        while (shots++ < shots_bullets)
        {
            // calculate spread
            float spreadAngle = Random.Range(-bulletSpread, bulletSpread);
            float rads = Mathf.Deg2Rad * ((spreadAngle > 0) ? spreadAngle : (360f + spreadAngle));
            float x = body.right.x, y = body.right.y;

            Vector3 bulletDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));

            // play animation

            // // //

            if (showDebugStuff) Debug.DrawLine(body.position, body.position + bulletDir * 50f, Color.red, delayBetweenShots_gun);

            if (bullet && bulletSpawn)
            {
                Bullet b = Instantiate(bullet, bulletSpawn).GetComponent<Bullet>();
                b.transform.Rotate(0f, 0f, Vector2.SignedAngle(body.right, bulletDir));
                b.gameObject.layer = StaticHelpers.EnemyProjectileLayer;
                b.Fly(bulletDir, bulletSpeed, gunDamage);

                //play muzzle effect
            }
            yield return new WaitForSeconds(delayBetweenShots_gun);
        }

        NextAttack();
    }
    
    private IEnumerator PerformLaserAttack()
    {
        Debug.Log("laser");
        yield return new WaitForSeconds(3);
        NextAttack();
    }

    private void NextAttack()
    {
        // if can see player -> next attack
        StartCoroutine(attackPattern.Dequeue());
        //StartCoroutine(nameof(PerformGunAttack));
    }

    private void NewAttackOrder()
    {                                                                                       // <- INFINITE LOOP
        for (int i = 0; i < allAttacks.Count; i++)
        {
            string newAttack = allAttacks[Random.Range(0, allAttacks.Count - 1)];
            while (attackPattern.Contains(newAttack))
            {
                 newAttack = allAttacks[Random.Range(0, allAttacks.Count - 1)];
            }
            attackPattern.Enqueue(newAttack);
            Debug.Log($"added {newAttack}");
        }
    }
}
