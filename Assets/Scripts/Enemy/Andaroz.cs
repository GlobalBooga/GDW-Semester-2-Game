using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using static Unity.Burst.Intrinsics.X86;

public class Andaroz : Enemy
{
    
    public Transform bulletSpawn;
    public GameObject muzzleFlash;
    public List<Transform> missileSpawns;
    public List<Transform> laserStart;
    public List<LineRenderer> lineRenderer;
    public Animator TorsoAnimator;
    public Animator LegsAnimator;

    private AndarozScriptableObject aso;
    private int prev; // the previous index for sequential firing
    private List<string> allAttacks = new() { nameof(PerformGunAttack), nameof(PerformLaserAttack), nameof(PerformMissileAttack), nameof(PerformSlamAttack), nameof(PerformSwipeAttack)};
    private Queue<string> attackPattern = new();
    

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
    
    internal override void Awake()
    {
        base.Awake();

        aso = (AndarozScriptableObject)eso;
    }

    internal override void Start()
    {
        base.Start();
    }

    internal override void Update()
    {
        base.Update();
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
        if (hp.GetHealth() > hp.maxHealth * aso.maxHpForUse_swipe)
        {
            NextAttack();
            yield break;
        }


        Debug.Log("swipe");
        bool attacked = false;   
        eso.maxAttackDistance = aso.maxAttackDistance_swipe;
        eso.minAttackDistance = aso.minAttackDistance_swipe;
        eso.comfortableAttackDist = aso.comfortableAttackDist_swipe;
        eso.attackMovementSpeed = eso.runSpeed = aso.attackMovementSpeed_swipe;
        eso.chasePlayer = true;
        NewAttackDistance();

        float time = 0f;

        while (!attacked)
        {
            time += Time.deltaTime;
            if (time >= aso.maxAttackTime_swipe) break;

            // else if we are in attack distance
            if (PlayerDistance <= eso.maxAttackDistance)
            {
                eso.attackMovementSpeed = 0f;
                Debug.Log("swiping");
                // play swipe animation

                if (TorsoAnimator) TorsoAnimator.Play(SWIPE_ATTACK);


                attacked = true;
                yield return new WaitForSeconds(aso.swipeApplyDmgDelay);

                // if were still within range of swipe
                if (PlayerDistance <= eso.maxAttackDistance)
                    StaticHelpers.ApplyDamage(playerLoc.gameObject, aso.swipeDamage);

                eso.attackMovementSpeed = eso.runSpeed;
            }



            yield return null;
        }
        
        yield return new WaitForSeconds(aso.delayBeforeIdle_swipe);
        if (TorsoAnimator) TorsoAnimator.Play(IDLE);
        yield return new WaitForSeconds(aso.delayBeforeNextAttack_swipe);
        ResetAttack();
    }

    private IEnumerator PerformSlamAttack()
    {
        if (hp.GetHealth() > hp.maxHealth * aso.maxHpForUse_slam)
        {
            NextAttack();
            yield break;
        }

        Debug.Log("slam");
        bool attacked = false;
        eso.maxAttackDistance = aso.maxAttackDistance_slam;
        eso.minAttackDistance = aso.minAttackDistance_slam;
        eso.comfortableAttackDist = aso.comfortableAttackDist_slam;
        eso.attackMovementSpeed = eso.runSpeed = aso.attackMovementSpeed_slam;
        eso.chasePlayer = aso.chasePlayer_slam;
        NewAttackDistance();

        float time = 0f;

        while (!attacked)
        {
            time += Time.deltaTime;
            if (time >= aso.maxAttackTime_slam) break;

            // else if we are in attack distance
            if (PlayerDistance <= eso.maxAttackDistance)
            {
                eso.attackMovementSpeed = 0f;

                // play swipe animation
                Debug.Log("slamming");


                if (TorsoAnimator) TorsoAnimator.Play(SLAM_ATTACK);



                attacked = true;
                yield return new WaitForSeconds(aso.slamApplyDmgDelay);

                // if were still within range of swipe

                // calculate aoe

                if (PlayerDistance <= eso.maxAttackDistance)
                    StaticHelpers.ApplyDamage(playerLoc.gameObject, aso.slamDamage);

                eso.attackMovementSpeed = eso.runSpeed;
            }

            yield return null;
        }

        yield return new WaitForSeconds(aso.delayBeforeIdle_slam);
        if (TorsoAnimator) TorsoAnimator.Play(IDLE);
        yield return new WaitForSeconds(aso.delayBeforeNextAttack_slam);
        ResetAttack();
        //NextAttack();
    }

    private IEnumerator PerformMissileAttack()
    {
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
        yield return new WaitForSeconds(aso.delayBeforeNextAttack_missiles);
        ResetAttack();
        //NextAttack();
    }

    private IEnumerator PerformGunAttack()
    {
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
        yield return new WaitForSeconds(aso.delayBeforeNextAttack_gun);
        //NextAttack();
        ResetAttack();
    }
    
    private IEnumerator PerformLaserAttack()
    {
        if (hp.GetHealth() > hp.maxHealth * aso.maxHpForUse_laser)
        {
            NextAttack();
            yield break;
        }

        Debug.Log("laser");
        eso.maxAttackDistance = aso.maxAttackDistance_laser;
        eso.minAttackDistance = aso.minAttackDistance_laser;
        eso.comfortableAttackDist = aso.comfortableAttackDist_laser;
        eso.chasePlayer = aso.chasePlayer_laser;
        NewAttackDistance();

        // animation - arms out

        if (TorsoAnimator) TorsoAnimator.Play(LASER_ATTACK);

        yield return new WaitForSeconds(aso.delayBeforeIdle_laser);


        if (TorsoAnimator) TorsoAnimator.Play(IDLE);
        yield return new WaitForSeconds(aso.delayBeforeNextAttack_laser);
        ResetAttack();
        //NextAttack();
    }


    // call in update
    void ShootLaser()
    {
        RaycastHit2D hit = Physics2D.Raycast(laserStart.position, body.up, sso.maxDist, sso.whatBlocksSight);
        if (hit)
        {
            DrawLaser(laserStart.position - transform.position, hit.point - (Vector2)transform.position);
        }
        else
        {
            DrawLaser(laserStart.position - transform.position, laserStart.position - transform.position + (body.up * sso.maxDist));
        }
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


    }

    public override void OnDied()
    {
        StopAllCoroutines();
        StartCoroutine(nameof(EndBossFight));
    }


    private IEnumerator EndBossFight()
    {
        if (TorsoAnimator) TorsoAnimator.Play(DEATH);
        yield return new WaitForSeconds(1.86f);

        // spawn body parts

        base.OnDied();
    }
}
