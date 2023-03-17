using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TrainBossScriptableObject;

public class TrainBoss : MonoBehaviour
{
    public TrainBossScriptableObject tso;
    public List<MultiBarrelMissileLauncher> missileLaunchers;
    public List<MachineGunTurret> turrets;
    public List<Flamethrower> flamethrowers;
    public BoxCollider2D troopSpawn;

    private List<string> allAttacks = new() { nameof(Turrets), nameof(Flamethrower), nameof(Missiles), nameof(Artillery), nameof(Troops) };
    private Queue<string> attackPattern = new();
    private HPComponent hp;
    private Transform playerLoc;
    private Rigidbody2D rb;
    private bool turretattack;
    private bool flamethrowerattack;
    private bool missileattack;
    private bool artilleryattack;
    private Animator animator;

    private float ogX;
    private int defartilleryshots = 3;
    private bool defartillerybool = false;
    private float defatkspeed = 1;
    private bool defflamebool = true;
    private bool resettingpos;
    private bool posreset;

    // animations
    const string TURRETS_EXTRACT = "TurretExtract";
    const string TURRETS_RETRACT = "TurretRetract";
    const string MISSILES_EXTRACT = "MissilesExtract";
    const string MISSILES_RETRACT = "MissilesRetract";
    const string FLAMETHROWER_SHOOT0 = "FlamethrowerShoot0";
    const string FLAMETHROWER_END0 = "FlamethrowerEnd0";
    const string FLAMETHROWER_SHOOT1 = "FlamethrowerShoot1";
    const string FLAMETHROWER_END1 = "FlamethrowerEnd1";
    const string FLAMETHROWER_EXTRACT = "FlamethrowerExtract";
    const string FLAMETHROWER_RETRACT = "FlamethrowerRetract";

    const int LAYER_FLAMETHROWERS = 0;
    const int LAYER_FIREZONE0 = 10;
    const int LAYER_FIREZONE1 = 11;
    const int LAYER_TURRETS = 1;
    const int LAYER_MISSILES = 2;

    private void Start()
    {
        hp = GetComponent<HPComponent>();
        if (hp) hp.OnHPZero = OnDied;

        hp.OnHit.Add(WeaponDestroyed);

        playerLoc = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        Invoke(nameof(NextAttack), 1);
        ogX = LevelManager.instance.CurrentScene.manager.transform.position.x;
    }

    private void Update()
    {
        if (turretattack)
        {
            // aim at player
            foreach (var item in turrets)
            {
                if (item.enabled)
                {
                    Vector2 target = playerLoc.position - item.mainUnit.position;
                    float angle = Vector3.SignedAngle(target, -item.mainUnit.right, Vector3.back);

                    item.mainUnit.rotation = Quaternion.Euler(0, 0, 
                        Mathf.Clamp(item.mainUnit.rotation.eulerAngles.z + 
                        (angle * Time.deltaTime * tso.rotationSpeed), 90-tso.maxAngle, 90+tso.maxAngle));
                }
            }
        }

        if (!flamethrowerattack)
        {
            // return to center
            float currentX = CenterofFireZone();

            if (Mathf.Abs(currentX - ogX) > 1f)
            {
                if (!resettingpos)
                {
                    resettingpos = true;
                    rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                    rb.freezeRotation = true;
                    posreset = false;
                }


                if (currentX - ogX > 0)
                {
                    rb.AddForce(Vector2.left * tso.moveSpeed * rb.mass, ForceMode2D.Force);
                }
                else
                {
                    rb.AddForce(Vector2.right * tso.moveSpeed * rb.mass, ForceMode2D.Force);
                }
            }
            else if (!posreset)
            {
                posreset = true;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
    }

    private void OnDestroy()
    {
        tso.artillery_shots = defartilleryshots;
        tso.artillery_useWithOtherAttacks = defartillerybool;
        tso.artillery_delayBeforeNextAttack = defatkspeed;
        tso.flamethrower_delayBeforeNextAttack = defatkspeed;
        tso.missile_delayBeforeNextAttack = defatkspeed;
        tso.turrets_delayBeforeNextAttack = defatkspeed;
        tso.flamethrower_useWithOtherAttacks = defflamebool;
    }

    private void OnDisable()
    {
        tso.artillery_shots = defartilleryshots;
        tso.artillery_useWithOtherAttacks = defartillerybool;
        tso.artillery_delayBeforeNextAttack = defatkspeed;
        tso.flamethrower_delayBeforeNextAttack = defatkspeed;
        tso.missile_delayBeforeNextAttack = defatkspeed;
        tso.turrets_delayBeforeNextAttack = defatkspeed;
        tso.flamethrower_useWithOtherAttacks = defflamebool;
    }

    private IEnumerator Turrets()
    {
        if (tso.turrets_maxHpForUse < hp.GetHealth())
        {
            NextAttack();
            yield break;
        }

        //Debug.Log("Turrets");

        // expose
        animator.Play(TURRETS_EXTRACT, LAYER_TURRETS);

        // show hpbars
        foreach (var item in turrets)
        {
            item.mainUnit.GetComponent<TrainWeapon>().Expose();
        }

        yield return new WaitForSeconds(tso.turrets_aimStartDelay);
        turretattack = true;

        yield return new WaitForSeconds(tso.turrets_shootStartDelay);

        for (int i = 0; i < tso.shots_Turrets; i++)
        {
            foreach (var item in turrets)
            {
                item.Fire(tso.bullet, tso.turrets_damage, tso.bulletSpread, 
                    tso.bulletSpeed, tso.turrets_coverDamageMultiplier);
            }

            yield return new WaitForSeconds(tso.turrets_delayBetweenShots);
        }

        turretattack = false;

        // retract
        int rotdone = 0;
        bool retract =false;

        while (rotdone <= turrets.Count)
        {
            foreach (var item in turrets)
            {
                Vector3 targetDir = -transform.up;
                float angle = Vector3.SignedAngle(targetDir, -item.mainUnit.right, Vector3.back);
                if (Mathf.Abs(angle) > 0.01f)
                {
                    item.mainUnit.Rotate(0, 0, angle * Time.deltaTime * tso.rotationSpeed);
                }
                else
                {
                    rotdone++;
                    item.mainUnit.rotation = Quaternion.Euler(0, 0, 90f);
                }

                if (Mathf.Abs(angle) < 1f && !retract)
                {
                    retract = true;
                    animator.Play(TURRETS_RETRACT, LAYER_TURRETS);

                    // hide hpbars
                    foreach (var t in turrets)
                    {
                        t.mainUnit.GetComponent<TrainWeapon>().Hide();
                    }
                }
                
            }
            yield return null;
        }




        yield return new WaitForSeconds(tso.turrets_delayBeforeNextAttack);
        NextAttack();
    }

    private IEnumerator Flamethrower()
    {
        if (tso.flamethrower_maxHpForUse < hp.GetHealth())
        {
            NextAttack();
            yield break;
        }

        flamethrowerattack = true;
        animator.Play(FLAMETHROWER_EXTRACT, LAYER_FLAMETHROWERS);

        // show hpbars
        foreach (var item in flamethrowers)
        {
            item.mainUnit.GetComponent<TrainWeapon>().Expose();
        }


        float time = 0;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        rb.freezeRotation = true;
        resettingpos = false;
        if (tso.aimForPlayer)
        {
            rb.drag = 1f;
            while (true)
            {
                Vector3 playerDirection = (playerLoc.position - transform.position);
                if (playerDirection.x > 0) playerDirection = Vector3.right;
                else playerDirection = Vector3.left;
                    
                if ((playerDirection.x < 0 && CenterofFireZone() > (ogX - tso.maxXmove)) ||
                    (playerDirection.x > 0 && CenterofFireZone() < (ogX + tso.maxXmove)))
                {
                    rb.AddForce(playerDirection * tso.moveSpeed * rb.mass, ForceMode2D.Force);
                }
                else
                {
                    break;
                }

                if (Mathf.Abs(playerLoc.position.x - CenterofFireZone()) < 5f)
                {
                    break;
                }

                time += Time.deltaTime;
                yield return null;
            }
            rb.drag = 5f;
        }
        else
        {
            Vector3 playerDirection = (playerLoc.position - transform.position);
            if (playerDirection.x > 0) playerDirection = Vector3.right;
            else playerDirection = Vector3.left;

            rb.drag = 1f;
            while (true)
            {
                if ((playerDirection.x < 0 && CenterofFireZone() > (ogX - tso.maxXmove)) ||
                   (playerDirection.x > 0 && CenterofFireZone() < (ogX + tso.maxXmove)))
                {
                    rb.AddForce(playerDirection * tso.moveSpeed * rb.mass, ForceMode2D.Force);
                }
                else
                {
                    break;
                }

                time += Time.deltaTime;
                yield return null;
            }
            rb.drag = 5f;
        }

        yield return new WaitForSeconds(tso.flamethrower_shootStartDelay);
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        

        // Shoot fire
        animator.Play(FLAMETHROWER_SHOOT0, LAYER_FIREZONE0);
        animator.Play(FLAMETHROWER_SHOOT1, LAYER_FIREZONE1);
        if (tso.flamethrower_useWithOtherAttacks) NextAttack();


        // Retract flamethrowers
        yield return new WaitForSeconds(tso.flamethrower_timeBeforeRetract);
        animator.Play(FLAMETHROWER_RETRACT, LAYER_FLAMETHROWERS);


        // hide hpbars
        foreach (var item in flamethrowers)
        {
            item.mainUnit.GetComponent<TrainWeapon>().Hide();
        }


        // end fire
        yield return new WaitForSeconds(tso.flamethrower_timeBeforeEnd);
        animator.Play(FLAMETHROWER_END0, LAYER_FIREZONE0);
        animator.Play(FLAMETHROWER_END1, LAYER_FIREZONE1);



        // this line is only here because otherwise it will give an error
        // when you start coding, move it to where you need it
        yield return new WaitForSeconds(tso.flamethrower_delayBeforeNextAttack);
        flamethrowerattack = false;
        if (!tso.flamethrower_useWithOtherAttacks) NextAttack();
    }

    private IEnumerator Missiles()
    {
        if (tso.missile_maxHpForUse < hp.GetHealth())
        {
            NextAttack();
            yield break;
        }

        //Debug.Log("missiles");

        missileattack = true;

        // make launchers come out
        animator.Play(MISSILES_EXTRACT, LAYER_MISSILES);

        // show hpbars
        foreach (var item in missileLaunchers)
        {
            item.mainUnit.GetComponent<TrainWeapon>().Expose();
        }

        yield return new WaitForSeconds(tso.missile_aimStartDelay);

        // play lock on animation
        if (tso.crosshairController && tso.missileRotForce > 0)
        {
            Crosshair crosshair = Instantiate(tso.crosshairController, transform).GetComponent<Crosshair>();
            if (crosshair)
            {
                crosshair.AimAt(playerLoc);
                tso.missile_shootStartDelay = crosshair.animationTime;
            }
        }

        yield return new WaitForSeconds(tso.missile_shootStartDelay);
        // shoot

        for (int n = 0; n < missileLaunchers.Count * 2; n++) 
        {
            int foo = 0;
            for (int i = 0; i < tso.shots_missiles; i++)
            {
                missileLaunchers[n % missileLaunchers.Count].FireAt(
                    playerLoc, tso.missile, tso.missile_damage, tso.missileSpread, 
                    tso.missileMaxSpeed, tso.missileRotForce, tso.missile_coverDamageMultiplier,
                    tso.whatTakesDamageFromMissiles, ref foo);

                yield return new WaitForSeconds(tso.missile_delayBetweenShots);
            }
        }
        yield return new WaitForSeconds(tso.missile_delaybeforeRetract);

        // make launchers return
        animator.Play(MISSILES_RETRACT, LAYER_MISSILES);

        // hide hpbars
        foreach (var item in missileLaunchers)
        {
            item.mainUnit.GetComponent<TrainWeapon>().Hide();
        }

        yield return new WaitForSeconds(tso.missile_delayBeforeNextAttack);
        missileattack = false;

        NextAttack();
    }

    private IEnumerator Artillery()
    {
        if (tso.Artillery_maxHpForUse < hp.GetHealth())
        {
            NextAttack();
            yield break;
        }

        //Debug.Log("artillery");

        artilleryattack = true;

        yield return new WaitForSeconds(tso.artillery_shootStartDelay);
        if (tso.artillery_useWithOtherAttacks) NextAttack();

        float maxOffset, xpos, ypos;

        // shots
        for (int i = 0; i < tso.artillery_shots; i++)
        {
            CameraShake.instance.ShakeCamera(tso.artillery_cameraShakeIntensity, tso.artillery_cameraShakeTime);
            yield return new WaitForSeconds(tso.artillery_delayBetweenShots);
        }


        // delay before they land
        yield return new WaitForSeconds(tso.artillery_airTime);

        for (int i = 0; i < tso.artillery_shots; i++)
        {
            ArtilleryStrike a = Instantiate(tso.artilleryStrike, playerLoc).GetComponent<ArtilleryStrike>();
            a.transform.parent = null;
            a.explosive.damage = tso.artillery_damage;
            a.impactDelay = tso.artillery_imactDelay;

            maxOffset = a.transform.GetChild(0).transform.localScale.x / 2 * (1 - tso.artillery_accuracy);
            xpos = Random.Range(-maxOffset, maxOffset) + a.transform.position.x;
            ypos = Random.Range(-maxOffset, maxOffset) + a.transform.position.y;
            
            a.transform.position = new Vector3(xpos, ypos, 0f);

            yield return new WaitForSeconds(tso.artillery_delayBetweenShots);
        }

        yield return new WaitForSeconds(tso.artillery_delayBeforeNextAttack);
        artilleryattack = false;
        if (!tso.artillery_useWithOtherAttacks) NextAttack();
    }
    
    private IEnumerator Troops()
    {
        if (tso.troopDeploy_maxHpForUse < hp.GetHealth())
        {
            NextAttack();
            yield break;
        }
        yield return new WaitForSeconds(tso.troop_startDelay);

        for (int i = 0; i < tso.troop_spawnAmount; i++)
        {
            Transform t = Instantiate(tso.troop_types[Random.Range(0, tso.troop_types.Length)], transform).transform;
            t.position = new Vector3(
                Random.Range(troopSpawn.bounds.min.x, troopSpawn.bounds.max.x),
                Random.Range(troopSpawn.bounds.min.y, troopSpawn.bounds.max.y), 0f);
        }




        yield return new WaitForSeconds(tso.troop_delayBeforeNextAttack);
        NextAttack();
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
   
    private void NextAttack()
    {
        if (!tso.enableTurrets && !tso.enableFlamethrower && !tso.enableMissiles && !tso.enableArtillery && !tso.enableTroops)
        {
            Debug.LogWarning("No attacks enabled! Enabling attack 1");
            tso.enableTurrets = true;
        }

        if (attackPattern.Count == 0) NewAttackOrder();

        // skip disabled attacks
        if (attackPattern.Peek() == nameof(Turrets) && (!tso.enableTurrets || turretattack || AreTurretsDestroyed())) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(Flamethrower) && (!tso.enableFlamethrower || flamethrowerattack || AreFlamethrowersDestroyed())) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(Missiles) && (!tso.enableMissiles || missileattack || AreMissileLaunchersDestroyed())) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(Artillery) && (!tso.enableArtillery || artilleryattack)) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(Troops) && !tso.enableTroops) { attackPattern.Dequeue(); NextAttack(); return; }

        // Start the attack coroutine
        StartCoroutine(attackPattern.Dequeue());
    }

    /// <summary>
    /// Called when hp is 0
    /// </summary>
    public void OnDied()
    {
        //StopAllCoroutines();
    }

    public void WeaponDestroyed() // AKA on hit
    {
        tso.artillery_shots++;

        if (hp.GetHealth() == 5)
        {
            tso.artillery_useWithOtherAttacks = true;
        }

        if (hp.GetHealth() == 1)
        {
            tso.artillery_useWithOtherAttacks = false;
            tso.flamethrower_useWithOtherAttacks = false;
        }

        // Speed up
        tso.artillery_delayBeforeNextAttack = Mathf.Clamp(tso.artillery_delayBeforeNextAttack - 0.25f, 0, 2);
        tso.flamethrower_delayBeforeNextAttack = Mathf.Clamp(tso.flamethrower_delayBeforeNextAttack - 0.25f, 0, 2);
        tso.missile_delayBeforeNextAttack = Mathf.Clamp(tso.missile_delayBeforeNextAttack - 0.25f, 0, 2);
        tso.turrets_delayBeforeNextAttack = Mathf.Clamp(tso.turrets_delayBeforeNextAttack - 0.25f, 0, 2);
    }

    public bool AreFlamethrowersDestroyed()
    {
        int i = 0;
        foreach (var item in flamethrowers)
        {
            if (!item.mainUnit.gameObject.activeSelf)
            {
                i++;
            }
        }
        return i == flamethrowers.Count;
    }

    public bool AreTurretsDestroyed()
    {
        int i = 0;
        foreach (var item in turrets)
        {
            if (!item.mainUnit.gameObject.activeSelf)
            {
                i++;
            }
        }
        return i == turrets.Count;
    }

    public bool AreMissileLaunchersDestroyed()
    {
        int i = 0;
        foreach (var item in missileLaunchers)
        {
            if (!item.mainUnit.gameObject.activeSelf)
            {
                i++;
            }
        }
        return i == missileLaunchers.Count;
    }

    private float CenterofFireZone()
    {
        int i = 0;
        foreach (var item in flamethrowers)
        {
            if (!item.mainUnit.gameObject.activeSelf)
            {
                i++;
            }
        }

        if (i == 0)
        {
            return (flamethrowers[0].mainUnit.position.x + flamethrowers[1].mainUnit.position.x) / 2;
        }
        else
        {
            foreach (var item in flamethrowers)
            {
                if (item.mainUnit.gameObject.activeSelf)
                {
                    return item.mainUnit.position.x;
                }
            }
        }

        //Debug.LogWarning("CenterofFireZone returned 0!");
        return ogX;
    }
}
