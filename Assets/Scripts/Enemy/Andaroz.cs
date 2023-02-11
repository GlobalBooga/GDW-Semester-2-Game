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
    public float delayBeforeNextAttack_swipe = 3f;
    public float maxAttackTime_swipe = 10f;
    [Range(0f, 1f)] public float maxHpForUse_swipe = 1f;

    [Space(10f)]
    [Header("Melee Attack 2 - AOE Ground Slam"), Space(5f)]
    public float slamDamage;
    public float slamCooldown = 1f;
    public float slamApplyDmgDelay = 0.5f;
    public float slamReach;
    public float delayBeforeNextAttack_slam = 3f;
    public float maxAttackTime_slam = 10f;
    [Range(0f, 1f)] public float maxHpForUse_slam = 1f;

    [Space(10f)]

    [Header("Ranged Attack 1 - Machine Gun"), Space(5f)]
    public float gunDamage;
    public float delayBetweenShots_gun = 0.5f;
    public float shootStartDelay_gun = 0.3f;
    public float delayBeforeNextAttack_gun = 3f;
    public float bulletSpeed = 15f;
    public float bulletSpread = 10f;
    public int shots_bullets = 50;
    public GameObject bullet;
    public Transform bulletSpawn;
    [Range(0f, 1f)] public float maxHpForUse_gun = 1f;

    [Space(10f)]

    [Header("Ranged Attack 2 - Missile Barrage"), Space(5f)]
    public float missileDamage;
    public float delayBetweenShots_missiles = 0.5f;
    public float shootStartDelay_missiles = 0.3f;
    public float delayBeforeNextAttack_missiles = 6f;
    public float missileSpeed = 7f;
    public float missileRotForce = 10f;
    public float missileMaxSpeed = 7f;
    public float missileSpread = 10f;
    public int shots_missiles = 10;
    public GameObject missile;
    public List<Transform> missileSpawns;
    public bool fireSequentially = true;
    private int prev; // the previous index for sequential firing
    public Crosshair crosshairController;
    [Range(0f, 1f)] public float maxHpForUse_missiles = 0.9f;

    [Space(10f)]

    [Header("Ranged Attack 3 - Twin Laser"), Space(5f)]
    public float laserDamage;
    public float attackDuration;
    public float delayBeforeNextAttack_laser = 3f;
    public float rotationSpeed;
    public float maxDist = 100f;
    public List<Transform> laserStart;
    public List<LineRenderer> lineRenderer;
    [Range(0f, 1f)] public float maxHpForUse_laser = 0.6f;

    [Space(10f)]

    [Header("Attack Pattern"), Space(5f)]
    public List<string> orderedAttacks;
    [Range(0f, 1f)] public float HpForStage2 = 0.5f;

    [Space(10f)]

    [Header("Balancing And Debugging"), Space(5f)]
    public bool enableSwipe = true;
    public bool enableSlam = true;
    public bool enableMachineGun = true;
    public bool enableMissiles = true;
    public bool enableLaser = true;


    private List<string> allAttacks = new() { nameof(PerformGunAttack), nameof(PerformLaserAttack), nameof(PerformMissileAttack), nameof(PerformSlamAttack), nameof(PerformSwipeAttack)};
    private Queue<string> attackPattern = new();


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
        NextAttack();
    }

    private IEnumerator PerformSwipeAttack()
    {
        if (hp.GetHealth() > hp.maxHealth * maxHpForUse_swipe)
        {
            NextAttack();
            yield break;
        }


        Debug.Log("swipe");
        bool attacked = false;   
        maxAttackDistance = 5f;
        minAttackDistance = 4f;
        comfortableAttackDist = 0f;
        attackMovementSpeed = runSpeed = 8f;
        chasePlayer = true;
        NewAttackDistance();

        float time = 0f;

        while (!attacked)
        {
            time += Time.deltaTime;
            if (time >= maxAttackTime_swipe) break;

            // else if we are in attack distance
            if (PlayerDistance <= maxAttackDistance)
            {
                attackMovementSpeed = 0f;
                Debug.Log("swiping");
                // play swipe animation
                attacked = true;
                yield return new WaitForSeconds(swipeApplyDmgDelay);

                // if were still within range of swipe
                if (PlayerDistance <= maxAttackDistance)
                    StaticHelpers.ApplyDamage(playerLoc.gameObject, swipeDamage);

                attackMovementSpeed = runSpeed;
            }



            yield return null;
        }
        
        yield return new WaitForSeconds(delayBeforeNextAttack_swipe);
        ResetAttack();
    }

    private IEnumerator PerformSlamAttack()
    {
        if (hp.GetHealth() > hp.maxHealth * maxHpForUse_slam)
        {
            NextAttack();
            yield break;
        }

        Debug.Log("slam");
        bool attacked = false;
        maxAttackDistance = 5f;
        minAttackDistance = 4f;
        comfortableAttackDist = 0f;
        attackMovementSpeed = runSpeed = 8f;
        chasePlayer = true;
        NewAttackDistance();

        float time = 0f;

        while (!attacked)
        {
            time += Time.deltaTime;
            if (time >= maxAttackTime_slam) break;

            // else if we are in attack distance
            if (PlayerDistance <= maxAttackDistance)
            {
                attackMovementSpeed = 0f;

                // play swipe animation
                Debug.Log("slamming");
                attacked = true;
                yield return new WaitForSeconds(slamApplyDmgDelay);

                // if were still within range of swipe

                // calculate aoe

                if (PlayerDistance <= maxAttackDistance)
                    StaticHelpers.ApplyDamage(playerLoc.gameObject, slamDamage);

                attackMovementSpeed = runSpeed;
            }

            yield return null;
        }

        yield return new WaitForSeconds(delayBeforeNextAttack_slam);
        ResetAttack();
        //NextAttack();
    }

    private IEnumerator PerformMissileAttack()
    {
        if (hp.GetHealth() > hp.maxHealth * maxHpForUse_missiles)
        {
            NextAttack();
            yield break;
        }

        // homing missile attack
        Debug.Log("missile");


        // set to stationary - with all seeing eye
        maxAttackDistance = 40f;
        minAttackDistance = 30f;
        comfortableAttackDist = 0f;
        chasePlayer = false;
        NewAttackDistance();

        // play lock on animation
        if (crosshairController)
        {
            crosshairController.AimAt(playerLoc);
            shootStartDelay_missiles = crosshairController.animationTime;
        }

        yield return new WaitForSeconds(shootStartDelay_missiles);
        int shots = 0;
        prev = 0;
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
                HomingMissile b;
                if (missileSpawns.Count == 1)
                {
                    b = Instantiate(missile, missileSpawns[0]).GetComponent<HomingMissile>();
                }
                else if (!fireSequentially)
                {
                    b = Instantiate(missile, missileSpawns[Random.Range(0, missileSpawns.Count)]).GetComponent<HomingMissile>();
                }
                else
                {
                    if (prev >= missileSpawns.Count) prev = 0;
                    b = Instantiate(missile, missileSpawns[prev++]).GetComponent<HomingMissile>();
                }

                b.transform.Rotate(0f, 0f, Vector2.SignedAngle(body.right, missileDir));
                b.gameObject.layer = StaticHelpers.EnemyMissile;
                b.Fly(playerLoc, missileDir, missileSpeed, missileRotForce, missileMaxSpeed, missileDamage);

                //play muzzle effect
            }
            yield return new WaitForSeconds(delayBetweenShots_missiles);
        }
        yield return new WaitForSeconds(delayBeforeNextAttack_missiles);
        ResetAttack();
        //NextAttack();
    }

    private IEnumerator PerformGunAttack()
    {
        if (hp.GetHealth() > hp.maxHealth * maxHpForUse_gun)
        {
            NextAttack();
            yield break;
        }

        // GUN ATTACK
        // in this attack, Andaroz chases the player at walking speed.
        // he is always facing the player
        Debug.Log("gun");

        maxAttackDistance = 25f;
        minAttackDistance = 20f;
        comfortableAttackDist = 15f;
        runSpeed = retreatSpeed = attackMovementSpeed = 3f;
        chasePlayer = true;
        NewAttackDistance();


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

        yield return new WaitForSeconds(delayBeforeNextAttack_gun);
        //NextAttack();
        ResetAttack();
    }
    
    private IEnumerator PerformLaserAttack()
    {
        if (hp.GetHealth() > hp.maxHealth * maxHpForUse_laser)
        {
            NextAttack();
            yield break;
        }

        Debug.Log("laser");
        maxAttackDistance = 40f;
        minAttackDistance = 30f;
        comfortableAttackDist = 0f;
        chasePlayer = false;
        NewAttackDistance();

        // animation - arms out


        yield return new WaitForSeconds(delayBeforeNextAttack_laser);
        ResetAttack();
        //NextAttack();
    }

    private void NextAttack()
    {
        if (!enableLaser && !enableMachineGun && !enableMissiles && !enableSlam && !enableSwipe) return;

        // if can see player -> next attack
        if (attackPattern.Count == 0) NewAttackOrder();

        // skip disabled attacks
        if (attackPattern.Peek() == nameof(PerformSwipeAttack) && !enableSwipe) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(PerformSlamAttack) && !enableSlam) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(PerformGunAttack) && !enableMachineGun) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(PerformMissileAttack) && !enableMissiles) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(PerformLaserAttack) && !enableLaser) { attackPattern.Dequeue(); NextAttack(); return; }

        StartCoroutine(attackPattern.Dequeue());
    }

    private void NewAttackOrder()
    {                                                                                     
        for (int i = 0; i < allAttacks.Count; i++)
        {
            string newAttack = allAttacks[Random.Range(0, allAttacks.Count)]; // using the int version - max is exclusive

            while (attackPattern.Contains(newAttack))
            {
                 newAttack = allAttacks[Random.Range(0, allAttacks.Count)];
            }

            attackPattern.Enqueue(newAttack);
        }

    }

    internal override void ResetAttack()
    {
        base.ResetAttack();

    }
}
