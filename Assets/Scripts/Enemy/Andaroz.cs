using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Andaroz : Enemy
{
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
    private const string LASER_ATTACK_LONG = "Andaroz_Lasers_FirstTime";
    private const string IDLE = "Andaroz_Idle";
    private const string WALK = "Andaroz_Walk";
    private const string RUN = "Andaroz_Run";
    private const string DEATH = "Andaroz_Death";
    private const string LEGS_WALK = "Andaroz_Legs_Walk";
    private const string LEGS_RUN = "Andaroz_Legs_Run";
    private const string LEGS_ATTACKSTANCE = "Andaroz_Legs_AttackStance";
    private const string LEGS_SHOOT = "Andaroz_Legs_Shoot";

    private bool stage2;

    private bool pauseMovement;
    //private bool scriptedMoment;

    private bool fight = false;

    private bool firstTime;
    private bool completeStage2;

    internal override void Awake()
    {
        base.Awake();
    }

    internal void OnEnable()
    {
        Invoke(nameof(StartBossFight), 2f);
    }

    internal override void OnDisable()
    {
        CancelInvoke();
    }

    internal override void Start()
    {
        base.Start();
        rightLaser = rightLR.gameObject.GetComponent<Laser>();
        leftLaser = leftLR.gameObject.GetComponent<Laser>();
        centerOfRoom = transform.parent.position;
        hp.OnHalfHP = Stage2;
        aso = (AndarozScriptableObject)eso;
    }

    internal override void Update()
    {
        // look at the player
        // go to the center of the room when on stage 2
        if (!goToCenterOfRoom && fight) base.Update();
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == StaticHelpers.SpecialBreakableObjectLayer)
        {
            StaticHelpers.ApplyDamage(collision.gameObject, eso.defaultMoveForce);
        }
    }

    internal override void FixedUpdate()
    {
        // walk towards the player
        if (!pauseMovement && fight)
        {
            // Standard chase player
            if (eso.chasePlayer && lockedOnPlayer && PlayerDistance > attackDistance)
            {
                ChasePlayer();
            }
            // if player is getting too close
            else if (lockedOnPlayer && PlayerDistance < comfortableAttackDist)
            {
                Retreat();
            }
            // if we are in the comfortable attack zone
            // or if we just havent discovered the player yet
            else
            {
                Stay();
            }
        }
    }

    internal override void Attack()
    {
        base.Attack();
        NextAttack();
    }

    private IEnumerator PerformSwipeAttack()
    {
        /*// prepare - look at player
        while (!LookAt(playerLoc.position)) yield return null;
*/
        if (hp.GetHealth() == 0) yield break;
       /* if (hp.GetHealth() > hp.maxHealth * aso.swipe_maxHpForUse)
        {
            NextAttack();
            yield break;
        }*/

        //Debug.Log("swipe");
        SetSwipeBehaviour();
        NewAttackDistance();

        if (LegsAnimator) LegsAnimator.Play(LEGS_RUN);


        float time = 0f;

        while (true)
        {
            //UpdateDetection();

            time += Time.deltaTime;
            if (time >= aso.swipe_maxAttackTime) break;
            //Debug.Log(time);
            // else if we are in attack distance
            if (PlayerDistance <= maxAttackDistance)
            {
                // Locking rotation
                if (aso.swipe_lockRotation) Invoke(nameof(LockRotation),aso.swipe_lockRotDelay);

                //Debug.Log("swiping");

                // play swipe animation
                if (LegsAnimator) LegsAnimator.Play(LEGS_ATTACKSTANCE);
                if (TorsoAnimator) TorsoAnimator.Play(SWIPE_ATTACK);


                yield return new WaitForSeconds(aso.swipe_applyDmgDelay);

                // if were still within range of swipe
                Collider2D[] cols = Physics2D.OverlapCircleAll(body.position, maxAttackDistance, aso.whatTakesDamage);
                if (cols.Length > 0)
                {
                    foreach (var item in cols)
                    {
                        if (Vector3.Angle(body.up, item.transform.position - transform.position) < aso.swipe_angle / 2f)
                        {
                            if (item.gameObject.layer == StaticHelpers.PlayerLayer)
                            {
                                StaticHelpers.ApplyDamage(item.gameObject, aso.swipe_damage);
                            }
                            else if (aso.swipe_pillarDamageMultiplier > 0)
                            {
                                StaticHelpers.ApplyDamage(item.gameObject, aso.swipe_damage * aso.swipe_pillarDamageMultiplier);
                            }
                        }
                    }
                }

                attackMovementSpeed = aso.swipe_attackMovementSpeed;

                break;
            }

            yield return null;
        }
        //Debug.Log("swipe end");

        yield return new WaitForSeconds(aso.swipe_delayBeforeIdle);
        // unlock rotation
        SetDefaultBehaviour();
        NewAttackDistance();
        if (LegsAnimator) LegsAnimator.Play(LEGS_RUN);
        if (TorsoAnimator) TorsoAnimator.Play(IDLE);
        if (!stage2) yield return new WaitForSeconds(aso.swipe_delayBeforeNextAttack);
        ResetAttack();
    }

    private void SetSwipeBehaviour()
    {
        runSpeed = aso.swipe_runSpeed;
        retreatSpeed = aso.swipe_retreatSpeed;
        minAttackDistance = aso.swipe_minAttackDistance;
        maxAttackDistance = aso.swipe_maxAttackDistance;
        attackMovementSpeed = aso.swipe_attackMovementSpeed;
        comfortableAttackDist = aso.swipe_comfortableAttackDist;
        eso.chasePlayer = aso.swipe_chasePlayer;
    }

    private IEnumerator PerformSlamAttack()
    {
        if (hp.GetHealth() > hp.maxHealth * aso.slam_maxHpForUse)
        {
            NextAttack();
            yield break;
        }

        //Debug.Log("slam");
        SetSlamBehaviour();
        NewAttackDistance();

        if (LegsAnimator) LegsAnimator.Play(LEGS_RUN);

        float time = 0f;

        while (true)
        {
            time += Time.deltaTime;
            if (time >= aso.slam_maxAttackTime) break;
            //Debug.Log(time);
         
            // else if we are in attack distance
            if (PlayerDistance <= maxAttackDistance)
            {
                //freeze rotation
                if (aso.slam_lockRotation) Invoke(nameof(LockRotation), aso.slam_lockRotDelay);

                // play swipe animation
                //Debug.Log("slamming");

                if (LegsAnimator) LegsAnimator.Play(LEGS_ATTACKSTANCE);
                if (TorsoAnimator) TorsoAnimator.Play(SLAM_ATTACK);

                yield return new WaitForSeconds(aso.slam_applyDmgDelay);

                Instantiate(aso.slam_Particles, slamPoint);

                CameraShake.instance.ShakeCamera(aso.slam_cameraShakeIntensity, aso.slam_cameraShakeTime);
                
                // Apply damage
                Collider2D[] cols = Physics2D.OverlapCircleAll(slamPoint.position,slamPoint.GetComponent<CircleCollider2D>().radius, aso.whatTakesDamage);
                if (cols.Length > 0) 
                {
                    foreach (var item in cols)
                    {
                        if (item.gameObject.layer == StaticHelpers.SpecialBreakableObjectLayer)
                        {
                            if (aso.slam_pillarDamageMultiplier > 0)
                            {
                                StaticHelpers.ApplyDamage(item.gameObject, aso.slam_damage * aso.slam_pillarDamageMultiplier);
                            }
                        }
                        else
                        {
                            StaticHelpers.ApplyDamage(item.gameObject, aso.slam_damage);
                        }
                    }
                }

                attackMovementSpeed = aso.swipe_attackMovementSpeed;

                break;
            }

            yield return null;
        }
        //Debug.Log("slam end");
        yield return new WaitForSeconds(aso.slam_delayBeforeIdle);
        SetDefaultBehaviour();
        NewAttackDistance();
        if (LegsAnimator) LegsAnimator.Play(LEGS_RUN);
        if (TorsoAnimator) TorsoAnimator.Play(IDLE);
        if (!stage2) yield return new WaitForSeconds(aso.slam_delayBeforeNextAttack);
        ResetAttack();
    }

    private void SetSlamBehaviour()
    {
        runSpeed = aso.slam_runSpeed;
        retreatSpeed = aso.slam_retreatSpeed;
        minAttackDistance = aso.slam_minAttackDistance;
        maxAttackDistance = aso.slam_maxAttackDistance;
        attackMovementSpeed = aso.slam_attackMovementSpeed;
        comfortableAttackDist = aso.slam_comfortableAttackDist;
        eso.chasePlayer = aso.slam_chasePlayer;
    }

    private IEnumerator PerformMissileAttack()
    {
        if (hp.GetHealth() == 0) yield break;
        if (hp.GetHealth() > hp.maxHealth * aso.missile_maxHpForUse)
        {
            NextAttack();
            yield break;
        }

        // homing missile attack
        //Debug.Log("missile");


        SetMissileBehaviour();
        NewAttackDistance();

        // play lock on animation
        if (aso.crosshairController)
        {
            Crosshair crosshair = Instantiate(aso.crosshairController,transform).GetComponent<Crosshair>();
            if (crosshair)
            {
                crosshair.AimAt(playerLoc);
                aso.missile_shootStartDelay = crosshair.animationTime;
            }
        }

        if (LegsAnimator) LegsAnimator.Play(LEGS_ATTACKSTANCE);
        if (TorsoAnimator) TorsoAnimator.Play(MISSILE_ATTACK);


        yield return new WaitForSeconds(aso.missile_shootStartDelay);
        int shots = 0;
        prev = 0;

        //freeze rotation
        if (aso.missile_lockRotation) Invoke(nameof(LockRotation), aso.missile_lockRotDelay);

        while (shots++ < aso.shots_missiles)
        {
            // calculate spread
            float spreadAngle = Random.Range(-aso.missileSpread, aso.missileSpread);
            float rads = Mathf.Deg2Rad * ((spreadAngle > 0) ? spreadAngle : (360f + spreadAngle));
            float x = body.up.x, y = body.up.y;

            Vector3 missileDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));


            if (showDebugStuff) Debug.DrawLine(body.position, body.position + missileDir * 50f, Color.red, aso.missile_delayBetweenShots);

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
                b.Fly(playerLoc, missileDir, aso.missileRotForce, aso.missileMaxSpeed, aso.missile_damage);
                b.specialObjectDamageMultiplier = aso.missile_pillarDamageMultiplier;
                b.explosive.SetWhatTakesDamage(aso.whatTakesDamageFromMissiles);
                //play muzzle effect
            }
            yield return new WaitForSeconds(aso.missile_delayBetweenShots);
        }

        yield return new WaitForSeconds(aso.missile_delayBeforeIdle);
        if (TorsoAnimator) TorsoAnimator.Play(IDLE);
        if (!stage2) yield return new WaitForSeconds(aso.missile_delayBeforeNextAttack);
        ResetAttack();
        SetDefaultBehaviour();
        NewAttackDistance();
    }

    private void SetMissileBehaviour()
    {
        runSpeed = aso.missile_runSpeed;
        retreatSpeed = aso.missile_retreatSpeed;
        minAttackDistance = aso.missile_minAttackDistance;
        maxAttackDistance = aso.missile_maxAttackDistance;
        attackMovementSpeed = aso.missile_attackMovementSpeed;
        comfortableAttackDist = aso.missile_comfortableAttackDist;
        eso.chasePlayer = aso.missile_chasePlayer;
    }

    private IEnumerator PerformGunAttack()
    {
        if (hp.GetHealth() == 0) yield break;
        if (hp.GetHealth() > hp.maxHealth * aso.gun_maxHpForUse)
        {
            NextAttack();
            yield break;
        }

        // GUN ATTACK
        // in this attack, Andaroz chases the player at walking speed.
        // he is always facing the player
        //Debug.Log("gun");

        SetGunBehaviour();
        NewAttackDistance();


        // animation
        if (LegsAnimator) LegsAnimator.Play(LEGS_SHOOT);
        if (TorsoAnimator) TorsoAnimator.Play(GUN_ATTACK);


        yield return new WaitForSeconds(aso.gun_shootStartDelay);
        int shots = 0;
        while (shots++ < aso.shots_bullets)
        {
            // calculate spread
            float spreadAngle = Random.Range(-aso.bulletSpread, aso.bulletSpread);
            float rads = Mathf.Deg2Rad * ((spreadAngle > 0) ? spreadAngle : (360f + spreadAngle));
            float x = body.up.x, y = body.up.y;

            Vector3 bulletDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));

            if (showDebugStuff) Debug.DrawLine(body.position, body.position + bulletDir * 50f, Color.red, aso.gun_delayBetweenShots);

            if (aso.bullet && bulletSpawn)
            {
                Bullet b = Instantiate(aso.bullet, bulletSpawn).GetComponent<Bullet>();
                b.transform.Rotate(0f, 0f, Vector2.SignedAngle(body.up, bulletDir));
                b.gameObject.layer = StaticHelpers.EnemyProjectileLayer;
                b.Fly(bulletDir, aso.bulletSpeed, aso.gun_damage);
                b.specialObjectDamageMultiplier = aso.gun_pillarDamageMultiplier;

                //play muzzle effect
                if (muzzleFlash) muzzleFlash.SetActive(true);
            }
            yield return new WaitForSeconds(aso.gun_delayBetweenShots);
        }

        yield return new WaitForSeconds(aso.gun_delayBeforeIdle);
        if (TorsoAnimator) TorsoAnimator.Play(IDLE);
        if (!stage2) yield return new WaitForSeconds(aso.gun_delayBeforeNextAttack);
        ResetAttack();
        SetDefaultBehaviour();
        NewAttackDistance();
    }

    private void SetGunBehaviour()
    {
        runSpeed = aso.gun_runSpeed;
        retreatSpeed = aso.gun_retreatSpeed;
        minAttackDistance = aso.gun_minAttackDistance;
        maxAttackDistance = aso.gun_maxAttackDistance;
        attackMovementSpeed = aso.gun_attackMovementSpeed;
        comfortableAttackDist = aso.gun_comfortableAttackDist;
        eso.chasePlayer = aso.gun_chasePlayer;
    }

    private IEnumerator PerformLaserAttack()
    {
        if (hp.GetHealth() > hp.maxHealth * aso.laser_maxHpForUse)
        {
            NextAttack();
            yield break;
        }
        Debug.Log("laser");

        rotationTime = 0;
        lockRotation = false;
        SetDefaultBehaviour();
        if (Vector3.Distance(transform.position, centerOfRoom) > 5f)
        {
            while (!LookAt(centerOfRoom)) yield return null;
        }

        LockRotation();
        
        // set center of room as goto position
        if (Vector3.Distance(transform.position, centerOfRoom) > 5f)
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
            rotationTime = 0f;

            // go to center of the room
            while (true)
            {
                SearchForPlayer();

                // if playerposes is empty, we are in the middle of the room
                Vector3 test;
                if (!playerPoses.TryPeek(out test)) break;

                yield return null;
            }
        }

        // stop
        rb.velocity = Vector2.zero;

        lockRotation = false;

        // look at player
        if (firstTime)
        {
            while (!LookAt(playerLoc.position)) yield return null;
        }

        lockRotation = true;

        //transform.position = centerOfRoom;
        SetLaserBehaviour();

        hp.isInvincible = true;

        // animation - arms out
        if (LegsAnimator) LegsAnimator.Play(LEGS_ATTACKSTANCE);
        if (!firstTime) TorsoAnimator.Play(LASER_ATTACK);
        else TorsoAnimator.Play(LASER_ATTACK_LONG);

        yield return new WaitForSeconds(aso.delayParticlesToLaser);

        rightLaser.TurnOn();
        leftLaser.TurnOn();

        if (!firstTime)
        {

            CameraShake.instance.ShakeCamera(aso.laser_cameraShakeIntensity, aso.laser_cameraShakeTime);
            for (float i = 0; i < aso.laser_duration; i += Time.deltaTime)
            {
                ShootLaser();
                yield return null;
            }
        }
        else
        {
            completeStage2 = true;
            firstTime = false;
            CameraShake.instance.ShakeCamera(aso.laser_cameraShakeIntensity_firstTime, aso.laser_cameraShakeTime_firstTime);
            for (float i = 0; i < aso.laser_duration_firstTime; i += Time.deltaTime)
            {
                ShootLaser();
                yield return null;
            }
        }

        rightLaser.TurnOff();
        leftLaser.TurnOff();
        
        hp.isInvincible = false;

        yield return new WaitForSeconds(aso.delayLaserEndToIdle);

        goToCenterOfRoom = false;
        rotationTime = 0f;

        if (TorsoAnimator) TorsoAnimator.Play(IDLE);
        if (!stage2) yield return new WaitForSeconds(aso.laser_delayBeforeNextAttack);
        ResetAttack();
        SetDefaultBehaviour();
        NewAttackDistance();
    }

    private void SetLaserBehaviour()
    {
        runSpeed = aso.laser_runSpeed;
        retreatSpeed = aso.laser_retreatSpeed;
        minAttackDistance = aso.laser_minAttackDistance;
        maxAttackDistance = aso.laser_maxAttackDistance;
        attackMovementSpeed = aso.laser_attackMovementSpeed;
        comfortableAttackDist = aso.laser_comfortableAttackDist;
        eso.chasePlayer = aso.laser_chasePlayer;
    }

    // call every frame
    void ShootLaser()
    {
        RaycastHit2D hitR = Physics2D.Raycast(rightLaserStart.position, -TorsoAnimator.transform.up, 100f, aso.whatBlocksSight);
        RaycastHit2D hitL = Physics2D.Raycast(leftLaserStart.position, TorsoAnimator.transform.up, 100f, aso.whatBlocksSight);
        if (hitR)
        {
            if (hitR.collider.gameObject.layer == StaticHelpers.PlayerLayer) StaticHelpers.ApplyDamage(hitR.collider.gameObject, aso.laser_damage);
            else if (aso.laser_pillarDamageMultiplier > 0) StaticHelpers.ApplyDamage(hitR.transform.gameObject, aso.laser_damage * aso.laser_pillarDamageMultiplier);
            rightLaser.DrawLaser(rightLaserStart.position, hitR.point);
        }
        if (hitL)
        {
            if (hitL.collider.gameObject.layer == StaticHelpers.PlayerLayer) StaticHelpers.ApplyDamage(hitL.collider.gameObject, aso.laser_damage);
            else if (aso.laser_pillarDamageMultiplier > 0) StaticHelpers.ApplyDamage(hitL.transform.gameObject, aso.laser_damage * aso.laser_pillarDamageMultiplier);
            leftLaser.DrawLaser(leftLaserStart.position, hitL.point);
        }
    }

    private void NextAttack()
    {
        if (!aso.enableLaser && !aso.enableMachineGun && !aso.enableMissiles && !aso.enableSlam && !aso.enableSwipe) return;

        // if can see player -> next attack
        if (attackPattern.Count == 0) NewAttackOrder();

        // skip disabled attacks
        if (attackPattern.Peek() == nameof(PerformSwipeAttack) && (!aso.enableSwipe || stage2)) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(PerformSlamAttack) && (!aso.enableSlam || stage2)) { attackPattern.Dequeue(); NextAttack(); return; }
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
        //SetDefaultBehaviour();
        //NewAttackDistance();
    }

    public override void OnDied()
    {
        StopAllCoroutines();
        LockRotation();
        rb.velocity = Vector2.zero;
        StartCoroutine(nameof(EndBossFight));
    }

    private void Stage2()
    {
        Debug.Log("stage 2");
        stage2 = true;
        pauseMovement = true;
        rotationTime = 0;
        hp.isInvincible = true;
        StopAllCoroutines();

        CameraShake.instance.LerpCameraSize(12f);
        CameraShake.instance.SetCameraFollow(transform);

        StartCoroutine(nameof(Stage2Routine));
    }

    private IEnumerator Stage2Routine()
    {
        yield return new WaitForSeconds(0.5f);

        pauseMovement = true;
        if (TorsoAnimator) TorsoAnimator.Play(IDLE);

        // Look at player
        rotationTime = 0;
        while (!LookAt(playerLoc.position)) yield return null;

        // pause player input for scripted event
        LevelManager.instance.DisablePlayerInput();

        Debug.Log("lets end this..");

        yield return new WaitForSeconds(0.5f);

        attackPattern.Clear();
        pauseMovement = false;
        firstTime = true;
        StartCoroutine(nameof(PerformLaserAttack));

        while(!completeStage2) yield return null;

        CameraShake.instance.LerpCameraSize(15f);
        CameraShake.instance.SetCameraFollow(Camera.main.transform);

        CameraShake.instance.RestoreCamPos(new Vector2(0, Mathf.Clamp(playerLoc.position.y, LevelManager.instance.GetCurrentLargeRoomBounds().w, LevelManager.instance.GetCurrentLargeRoomBounds().z)), 1);
        yield return new WaitForSeconds(1f);

        // resume player input
        LevelManager.instance.EnablePlayerInput();
    }

    private void StartBossFight()
    {
        fight = true;
        LevelManager.instance.EnablePlayerInput();
        LevelManager.instance.startBossBattle = true;
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

    private void LockRotation()
    {
        playerPoses.Clear();
        eso.chasePlayer = false;
        attackMovementSpeed = 0f;
        rb.velocity = Vector2.zero;

        lockRotation = true;
    }
}
