using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using static TrainBossScriptableObject;

public class TrainBoss : MonoBehaviour
{
    public TrainBossScriptableObject tso;
    public List<MultiBarrelMissileLauncher> missileLaunchers;
    public List<MachineGunTurret> turrets;
    public GameObject flamethrower;

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

    // animations

    const string TURRETS_EXTRACT = "TurretExtract";
    const string TURRETS_RETRACT = "TurretRetract";
    const string MISSILES_EXTRACT = "MissilesExtract";
    const string MISSILES_RETRACT = "MissilesRetract";
    const string FLAMETHROWER_SHOOT = "FlamethrowerShoot";
    const string FLAMETHROWER_END = "FlamethrowerEnd";
    const string FLAMETHROWER_EXTRACT = "FlamethrowerExtract";
    const string FLAMETHROWER_RETRACT = "FlamethrowerRetract";

    const int LAYER_TURRETS = 1;
    const int LAYER_MISSILES = 2;
    const int LAYER_FLAMETHROWER = 0;


    private void Start()
    {
        hp = GetComponent<HPComponent>();
        if (hp) hp.OnHPZero = OnDied;
        playerLoc = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        Invoke(nameof(NextAttack), 1);
        ogX = transform.position.x;
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
    }

    private IEnumerator Turrets()
    {
        if (!CanUseAttack(tso.turrets_maxHpForUse) && turretattack)
        {
            NextAttack();
            yield break;
        }

        Debug.Log("Turrets");

        // expose
        animator.Play(TURRETS_EXTRACT, LAYER_TURRETS);


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
                    Debug.Log(rotdone);
                }

                if (Mathf.Abs(angle) < 1f && !retract)
                {
                    retract = true;
                    animator.Play(TURRETS_RETRACT, LAYER_TURRETS);
                }
                
            }
            yield return null;
        }


        yield return new WaitForSeconds(tso.turrets_delayBeforeNextAttack);
        NextAttack();
    }

    private IEnumerator Flamethrower()
    {
        if (!CanUseAttack(tso.flamethrower_maxHpForUse) && flamethrowerattack)
        {
            NextAttack();
            yield break;
        }

        NextAttack();

        flamethrowerattack = true;
        animator.Play(FLAMETHROWER_EXTRACT, LAYER_FLAMETHROWER);

        if (rb)
        {
            float time = 0;
            if (tso.aimForPlayer)
            {
                rb.drag = 1f;
                while (true)
                {
                    Vector3 playerDirection = (playerLoc.position - transform.position).normalized;
                    
                    if ((playerDirection.x < 0 && transform.position.x > (ogX - tso.maxXmove)) ||
                     (playerDirection.x > 0 && transform.position.x < (ogX + tso.maxXmove)))
                    {
                        rb.AddForce(Vector2.right * playerDirection.x * tso.moveSpeed * rb.mass, ForceMode2D.Force);
                    }
                    else
                    {
                        break;
                    }

                    if (Mathf.Abs(playerLoc.position.x - flamethrower.transform.position.x) < 5f)
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
                Vector3 playerDirection = (playerLoc.position - transform.position).normalized;
                rb.drag = 1f;
                while (true)
                {
                    if ((playerDirection.x < 0 && transform.position.x > (ogX - tso.maxXmove))||
                     (playerDirection.x > 0 && transform.position.x < (ogX + tso.maxXmove)))
                    {
                        rb.AddForce(Vector2.right * playerDirection.x * tso.moveSpeed * rb.mass, ForceMode2D.Force);
                    }
                    else
                    {
                        break;
                    }

                    yield return null;
                }
                rb.drag = 5f;
            }
        }

        yield return new WaitForSeconds(tso.flamethrower_shootStartDelay);

        animator.Play(FLAMETHROWER_SHOOT, LAYER_FLAMETHROWER);

        yield return new WaitForSeconds(tso.time_flamethrower);
        //animator.Play(FLAMETHROWER_END);

        animator.Play(FLAMETHROWER_RETRACT, LAYER_FLAMETHROWER);


        // this line is only here because otherwise it will give an error
        // when you start coding, move it to where you need it
        yield return new WaitForSeconds(tso.flamethrower_delayBeforeNextAttack);
        flamethrowerattack = false;

        NextAttack();
    }

    private IEnumerator Missiles()
    {
        if (!CanUseAttack(tso.missile_maxHpForUse) && missileattack)
        {
            NextAttack();
            yield break;
        }

        Debug.Log("missiles");

        missileattack = true;

        // make launchers come out
        animator.Play(MISSILES_EXTRACT, LAYER_MISSILES);

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
        

        yield return new WaitForSeconds(tso.missile_delayBeforeNextAttack);
        missileattack = false;

        NextAttack();
    }

    private IEnumerator Artillery()
    {
        if (!CanUseAttack(tso.Artillery_maxHpForUse) && artilleryattack)
        {
            NextAttack();
            yield break;
        }

        Debug.Log("artillery");

        artilleryattack = true;

        yield return new WaitForSeconds(tso.artillery_shootStartDelay);

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
        NextAttack();
    }
    
    private IEnumerator Troops()
    {
        if (!CanUseAttack(tso.troopDeploy_maxHpForUse))
        {
            NextAttack();
            yield break;
        }

        yield return null;
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
        if (attackPattern.Peek() == nameof(Turrets) && !tso.enableTurrets) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(Flamethrower) && !tso.enableFlamethrower) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(Missiles) && !tso.enableMissiles) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(Artillery) && !tso.enableArtillery) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(Troops) && !tso.enableTroops) { attackPattern.Dequeue(); NextAttack(); return; }

        // Start the attack coroutine
        StartCoroutine(attackPattern.Dequeue());
    }

    /// <summary>
    /// Called when hp is 0
    /// </summary>
    public void OnDied()
    {
        StopAllCoroutines();
    }

    private bool CanUseAttack(float hpThreshold)
    {
        if (hp.GetHealth() == 0 || hp.GetHealth() > hp.maxHealth * hpThreshold) return false;
        else return true;
    }

}
