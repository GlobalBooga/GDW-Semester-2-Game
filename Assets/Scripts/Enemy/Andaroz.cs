using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Andaroz : Enemy
{
    public const string title = "ANDAROZ THE TECHNOLOGICAL MANIAC";

    public Transform bulletSpawn;
    public GameObject muzzleFlash;
    public List<Transform> missileSpawns;
    public Transform rightLaserStart;
    public Transform leftLaserStart;
    public LineRenderer rightLR;
    public LineRenderer leftLR;
    public Animator TorsoAnimator;
    public Animator LegsAnimator;
    public Transform slamPoint;
    public Transform legs;
    public GameObject deathExplosive;
    public List<Transform> deathExplosionTransforms;

    private AndarozScriptableObject aso;
    private int prev; // the previous index for sequential firing
    private List<string> allAttacks = new() { nameof(PerformGunAttack), nameof(PerformLaserAttack), nameof(PerformMissileAttack), nameof(PerformSlamAttack), nameof(PerformSwipeAttack)};
    private Queue<string> attackPattern = new();
    private Laser rightLaser;
    private Laser leftLaser;

    private bool goToCenterOfRoom;

    private Vector3 centerOfRoom;

    // ANIMATION KEYWORDS

    private const string SWIPE_ATTACK = "Andaroz_Swipe";
    private const string SLAM_ATTACK = "Andaroz_Slam";
    private const string GUN_ATTACK = "Andaroz_Shoot";
    private const string MISSILE_ATTACK = "Andaroz_Missiles";
    private const string LASER_ATTACK = "Andaroz_Lasers";
    private const string IDLE = "Andaroz_Idle";
    private const string WALK = "Andaroz_Walk";
    private const string RUN = "Andaroz_Run";
    private const string DEATH = "Andaroz_Death";
    private const string LEGS_WALK = "Andaroz_Legs_Walk";
    private const string LEGS_RUN = "Andaroz_Legs_Run";
    private const string LEGS_ATTACKSTANCE = "Andaroz_Legs_AttackStance";
    private const string LEGS_SHOOT = "Andaroz_Legs_Shoot";

    private float maxAttackDistance_default = 25f;
    private float minAttackDistance_default = 23f;
    private float runSpeed_default = 5f;

    private bool stage2;
    private bool freezelegs;
    private bool gunattack;

    internal override void Awake()
    {
        base.Awake();

        aso = (AndarozScriptableObject)eso;
    }

    internal override void Start()
    {
        base.Start();
        rightLaser = rightLR.gameObject.GetComponent<Laser>();
        leftLaser = leftLR.gameObject.GetComponent<Laser>();
        aso.viewDistance = 60;
        centerOfRoom = transform.parent.position;
        hp.OnHalfHP = Stage2;
    }

    internal override void Update()
    {
        if (!goToCenterOfRoom) base.Update();
        if (rb.velocity.magnitude > 0f && !isAttacking)
        {
            //if (TorsoAnimator) TorsoAnimator.Play(WALK);
            if (gunattack)
            {
                if (LegsAnimator) LegsAnimator.Play(LEGS_WALK);
            }
            else
            {
                if (LegsAnimator) LegsAnimator.Play(LEGS_RUN);
            }
        }

        //if (!freezelegs) legs.rotation = body.rotation;
    }

    internal override void FixedUpdate()
    {
        if (!goToCenterOfRoom) base.FixedUpdate();
        
    }

    internal override void Attack()
    {
        base.Attack();
        NextAttack();
    }

    private IEnumerator PerformSwipeAttack()
    {
        if (hp.GetHealth() == 0) yield break;
        if (hp.GetHealth() > hp.maxHealth * aso.maxHpForUse_swipe)
        {
            NextAttack();
            yield break;
        }


        Debug.Log("swipe");
        eso.maxAttackDistance = aso.maxAttackDistance_swipe;
        eso.minAttackDistance = aso.minAttackDistance_swipe;
        eso.comfortableAttackDist = aso.comfortableAttackDist_swipe;
        eso.attackMovementSpeed = eso.runSpeed = aso.attackMovementSpeed_swipe;
        eso.chasePlayer = true;
        NewAttackDistance();

        if (LegsAnimator) LegsAnimator.Play(LEGS_RUN);


        float time = 0f;

        while (true)
        {
            time += Time.deltaTime;
            if (time >= aso.maxAttackTime_swipe) break;
            Debug.Log(time);
            // else if we are in attack distance
            if (PlayerDistance <= eso.maxAttackDistance)
            {
                eso.attackMovementSpeed = 0f;
                Debug.Log("swiping");
                // play swipe animation
                freezelegs = true;
                if (LegsAnimator) LegsAnimator.Play(LEGS_ATTACKSTANCE);
                if (TorsoAnimator) TorsoAnimator.Play(SWIPE_ATTACK);


                yield return new WaitForSeconds(aso.swipeApplyDmgDelay);

                // if were still within range of swipe
                if (PlayerDistance <= eso.maxAttackDistance)
                    StaticHelpers.ApplyDamage(playerLoc.gameObject, aso.swipeDamage);

                eso.attackMovementSpeed = eso.runSpeed;

                break;
            }

            yield return null;
        }
        Debug.Log("swipe end");

        yield return new WaitForSeconds(aso.delayBeforeIdle_swipe);
        freezelegs = false;
        if (LegsAnimator) LegsAnimator.Play(LEGS_RUN);
        if (TorsoAnimator) TorsoAnimator.Play(IDLE);
        if (!stage2) yield return new WaitForSeconds(aso.delayBeforeNextAttack_swipe);
        ResetAttack();
    }

    private IEnumerator PerformSlamAttack()
    {
        if (hp.GetHealth() == 0) yield break;
        if (hp.GetHealth() > hp.maxHealth * aso.maxHpForUse_slam)
        {
            NextAttack();
            yield break;
        }

        Debug.Log("slam");
        eso.maxAttackDistance = aso.maxAttackDistance_slam;
        eso.minAttackDistance = aso.minAttackDistance_slam;
        eso.comfortableAttackDist = aso.comfortableAttackDist_slam;
        eso.attackMovementSpeed = eso.runSpeed = aso.attackMovementSpeed_slam;
        eso.chasePlayer = aso.chasePlayer_slam;
        NewAttackDistance();

        if (LegsAnimator) LegsAnimator.Play(LEGS_RUN);

        float time = 0f;

        while (true)
        {
            time += Time.deltaTime;
            if (time >= aso.maxAttackTime_slam) break;
            Debug.Log(time);
            // else if we are in attack distance
            if (PlayerDistance <= eso.maxAttackDistance)
            {
                eso.attackMovementSpeed = 0f;

                // play swipe animation
                Debug.Log("slamming");

                freezelegs = true;

                if (LegsAnimator) LegsAnimator.Play(LEGS_ATTACKSTANCE);
                if (TorsoAnimator) TorsoAnimator.Play(SLAM_ATTACK);

                yield return new WaitForSeconds(aso.slamApplyDmgDelay);

                Instantiate(aso.slamParticles, slamPoint);

                Collider2D col = Physics2D.OverlapCircle(slamPoint.position,slamPoint.GetComponent<CircleCollider2D>().radius, aso.whatIsPlayer);
                if (col) StaticHelpers.ApplyDamage(col.gameObject, aso.slamDamage);

                eso.attackMovementSpeed = eso.runSpeed;

                break;
            }

            yield return null;
        }
        Debug.Log("slam end");
        yield return new WaitForSeconds(aso.delayBeforeIdle_slam);
        freezelegs = false;
        if (LegsAnimator) LegsAnimator.Play(LEGS_RUN);
        if (TorsoAnimator) TorsoAnimator.Play(IDLE);
        if (!stage2) yield return new WaitForSeconds(aso.delayBeforeNextAttack_slam);
        ResetAttack();
        //NextAttack();
    }

    private IEnumerator PerformMissileAttack()
    {
        if (hp.GetHealth() == 0) yield break;
        if (hp.GetHealth() > hp.maxHealth * aso.maxHpForUse_missiles)
        {
            NextAttack();
            yield break;
        }

        // homing missile attack
        Debug.Log("missile");


        // set to stationary - with all seeing eye
        eso.maxAttackDistance = aso.maxAttackDistance_missiles;
        eso.minAttackDistance = aso.minAttackDistance_missiles;
        eso.comfortableAttackDist = aso.comfortableAttackDist_missiles;
        eso.chasePlayer = aso.chasePlayer_missiles;
        NewAttackDistance();

        // play lock on animation
        if (aso.crosshairController)
        {
            Crosshair crosshair = Instantiate(aso.crosshairController,transform).GetComponent<Crosshair>();
            if (crosshair)
            {
                crosshair.AimAt(playerLoc);
                aso.shootStartDelay_missiles = crosshair.animationTime;
            }
        }

        if (LegsAnimator) LegsAnimator.Play(LEGS_ATTACKSTANCE);
        if (TorsoAnimator) TorsoAnimator.Play(MISSILE_ATTACK);


        yield return new WaitForSeconds(aso.shootStartDelay_missiles);
        int shots = 0;
        prev = 0;
        while (shots++ < aso.shots_missiles)
        {
            // calculate spread
            float spreadAngle = Random.Range(-aso.missileSpread, aso.missileSpread);
            float rads = Mathf.Deg2Rad * ((spreadAngle > 0) ? spreadAngle : (360f + spreadAngle));
            float x = body.up.x, y = body.up.y;

            Vector3 missileDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));


            if (showDebugStuff) Debug.DrawLine(body.position, body.position + missileDir * 50f, Color.red, aso.delayBetweenShots_missiles);

            if (aso.missile && missileSpawns.Count > 0)
            {
                HomingMissile b;
                if (missileSpawns.Count == 1)
                {
                    b = Instantiate(aso.missile, missileSpawns[0]).GetComponent<HomingMissile>();
                }
                else if (!aso.fireSequentially)
                {
                    b = Instantiate(aso.missile, missileSpawns[Random.Range(0, missileSpawns.Count)]).GetComponent<HomingMissile>();
                }
                else
                {
                    if (prev >= missileSpawns.Count) prev = 0;
                    b = Instantiate(aso.missile, missileSpawns[prev++]).GetComponent<HomingMissile>();
                }

                b.transform.Rotate(0f, 0f, Vector2.SignedAngle(body.up, missileDir));
                b.gameObject.layer = StaticHelpers.EnemyMissile;
                b.Fly(playerLoc, missileDir, aso.missileSpeed, aso.missileRotForce, aso.missileMaxSpeed, aso.missileDamage);

                //play muzzle effect
            }
            yield return new WaitForSeconds(aso.delayBetweenShots_missiles);
        }

        yield return new WaitForSeconds(aso.delayBeforeIdle_missiles);
        if (TorsoAnimator) TorsoAnimator.Play(IDLE);
        if (!stage2) yield return new WaitForSeconds(aso.delayBeforeNextAttack_missiles);
        ResetAttack();
        //NextAttack();
    }

    private IEnumerator PerformGunAttack()
    {
        if (hp.GetHealth() == 0) yield break;
        if (hp.GetHealth() > hp.maxHealth * aso.maxHpForUse_gun)
        {
            NextAttack();
            yield break;
        }

        // GUN ATTACK
        // in this attack, Andaroz chases the player at walking speed.
        // he is always facing the player
        Debug.Log("gun");

        eso.maxAttackDistance = aso.maxAttackDist_gun;
        eso.minAttackDistance = aso.minAttackDistance_gun;
        eso.comfortableAttackDist = aso.comfortableAttackDist_gun;
        eso.runSpeed = eso.retreatSpeed = eso.attackMovementSpeed = aso.attackMovementSpeed_gun;
        eso.chasePlayer = aso.chasePlayer_gun;
        NewAttackDistance();


        // animation
        gunattack = true;
        freezelegs = true;
        if (LegsAnimator) LegsAnimator.Play(LEGS_SHOOT);
        if (TorsoAnimator) TorsoAnimator.Play(GUN_ATTACK);


        yield return new WaitForSeconds(aso.shootStartDelay_gun);
        int shots = 0;
        while (shots++ < aso.shots_bullets)
        {
            // calculate spread
            float spreadAngle = Random.Range(-aso.bulletSpread, aso.bulletSpread);
            float rads = Mathf.Deg2Rad * ((spreadAngle > 0) ? spreadAngle : (360f + spreadAngle));
            float x = body.up.x, y = body.up.y;

            Vector3 bulletDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));

            if (showDebugStuff) Debug.DrawLine(body.position, body.position + bulletDir * 50f, Color.red, aso.delayBetweenShots_gun);

            if (aso.bullet && bulletSpawn)
            {
                Bullet b = Instantiate(aso.bullet, bulletSpawn).GetComponent<Bullet>();
                b.transform.Rotate(0f, 0f, Vector2.SignedAngle(body.up, bulletDir));
                b.gameObject.layer = StaticHelpers.EnemyProjectileLayer;
                b.Fly(bulletDir, aso.bulletSpeed, aso.gunDamage);

                //play muzzle effect
                if (muzzleFlash) muzzleFlash.SetActive(true);
            }
            yield return new WaitForSeconds(aso.delayBetweenShots_gun);
        }

        yield return new WaitForSeconds(aso.delayBeforeIdle_gun);
        if (TorsoAnimator) TorsoAnimator.Play(IDLE);
        if (!stage2) yield return new WaitForSeconds(aso.delayBeforeNextAttack_gun);
        //NextAttack();

        gunattack = false;
        ResetAttack();
    }
    
    private IEnumerator PerformLaserAttack()
    {
        if (hp.GetHealth() == 0) yield break;

        if (hp.GetHealth() > hp.maxHealth * aso.maxHpForUse_laser)
        {
            NextAttack();
            yield break;
        }
        Debug.Log("laser");

        float temp = aso.viewDistance;
        aso.viewDistance = 0f;
        rotationTime = 0f;

        if (Vector3.Distance(transform.position, centerOfRoom) > 3f)
        {
            playerPoses.Clear();
            playerPoses.Enqueue(centerOfRoom);
            aso.chasePlayer = true;
            foundPlayer = false;
            nextPos = centerOfRoom;
            totalTravelDist = Vector3.Distance(transform.position, nextPos);
            lerpStartTime = Time.time;
            lerpStart = transform.position;
            goToCenterOfRoom = true;
        }

        // go to center of the room
        while (true)
        {
            base.FixedUpdate();

            LookAt(centerOfRoom);

            // if playerposes is empty, we are in the middle of the room
            Vector3 test;
            if (!playerPoses.TryPeek(out test)) break;

            yield return null;
        }

        // stop
        rb.velocity = Vector2.zero;
        transform.position = centerOfRoom;
        aso.maxAttackDistance = aso.maxAttackDistance_laser;
        aso.minAttackDistance = aso.minAttackDistance_laser;
        aso.comfortableAttackDist = aso.comfortableAttackDist_laser;
        aso.chasePlayer = aso.chasePlayer_laser;


        // animation - arms out
        if (LegsAnimator) LegsAnimator.Play(LEGS_ATTACKSTANCE);
        if (TorsoAnimator) TorsoAnimator.Play(LASER_ATTACK);

        yield return new WaitForSeconds(aso.delayParticlesToLaser);

        rightLaser.TurnOn();
        leftLaser.TurnOn();

        for (int i = 0; i < aso.laserDurationFrames; i++)
        {
            ShootLaser();
            yield return null;  
        }

        rightLaser.TurnOff();
        leftLaser.TurnOff();
        yield return new WaitForSeconds(aso.delayLaserEndToIdle);

        goToCenterOfRoom = false;
        aso.viewDistance = temp;
        rotationTime = 0f;

        if (TorsoAnimator) TorsoAnimator.Play(IDLE);
        if (!stage2) yield return new WaitForSeconds(aso.delayBeforeNextAttack_laser);
        ResetAttack();
        //NextAttack();
    }


    // call in update
    void ShootLaser()
    {
        RaycastHit2D hitR = Physics2D.Raycast(rightLaserStart.position, -TorsoAnimator.transform.up, 100f, aso.whatBlocksSight);
        RaycastHit2D hitL = Physics2D.Raycast(leftLaserStart.position, TorsoAnimator.transform.up, 100f, aso.whatBlocksSight);
        if (hitR)
        {
            if (hitR.collider.gameObject.layer == StaticHelpers.PlayerLayer) StaticHelpers.ApplyDamage(hitR.collider.gameObject, aso.laserDamage); 
            rightLaser.DrawLaser(rightLaserStart.position, hitR.point);
        }
        if (hitL)
        {
            if (hitL.collider.gameObject.layer == StaticHelpers.PlayerLayer) StaticHelpers.ApplyDamage(hitL.collider.gameObject, aso.laserDamage); 
            leftLaser.DrawLaser(leftLaserStart.position, hitL.point);
        }

        Physics2D.CircleCastAll(transform.position, 5, transform.up);
    }


    private void NextAttack()
    {
        if (!aso.enableLaser && !aso.enableMachineGun && !aso.enableMissiles && !aso.enableSlam && !aso.enableSwipe) return;

        // if can see player -> next attack
        if (attackPattern.Count == 0) NewAttackOrder();

        // skip disabled attacks
        if (attackPattern.Peek() == nameof(PerformSwipeAttack) && !aso.enableSwipe) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(PerformSlamAttack) && !aso.enableSlam) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(PerformGunAttack) && !aso.enableMachineGun) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(PerformMissileAttack) && !aso.enableMissiles) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(PerformLaserAttack) && !aso.enableLaser) { attackPattern.Dequeue(); NextAttack(); return; }

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
        aso.maxAttackDistance = maxAttackDistance_default;
        aso.minAttackDistance = minAttackDistance_default;
        aso.comfortableAttackDist = 3f;
        aso.runSpeed = runSpeed_default;
        aso.retreatSpeed = 3f;
        NewAttackDistance();

    }

    public override void OnDied()
    {
        rb.velocity = Vector2.zero;
        StopAllCoroutines();
        aso.viewDistance = 0f;
        StartCoroutine(nameof(EndBossFight));
    }


    private void Stage2()
    {
        Debug.Log("stage 2");
        stage2 = true;

        
    }

    private IEnumerator EndBossFight()
    {
        if (TorsoAnimator) TorsoAnimator.Play(DEATH);

        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(1.86f / 2);

        foreach(var thing in deathExplosionTransforms)
        {
            GameObject g = Instantiate(deathExplosive, thing);
            g.transform.parent = null;
            Destroy(g, 1f);
        }

        // spawn body parts
        Rigidbody2D[] rbs = Instantiate(aso.deadMe, transform).GetComponentsInChildren<Rigidbody2D>(true);
        foreach (var item in rbs)
        {
            item.transform.parent.parent = null;
            item.AddForce(new Vector2(Random.Range(0.2f,1f), Random.Range(0.2f, 1f)) * Random.Range(4f, 8f)*15f, ForceMode2D.Impulse);
            item.AddTorque(Random.Range(1f,5f),ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(0.5f / 2);

        base.OnDied();
    }
}
