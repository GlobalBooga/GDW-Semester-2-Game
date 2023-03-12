using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TrainBossScriptableObject;

public class TrainBoss : MonoBehaviour
{
    public TrainBossScriptableObject tso;
    public List<MultiBarrelMissileLauncher> missileLaunchers;
    public List<MachineGunTurret> turrets;

    private List<string> allAttacks = new() { nameof(Turrets), nameof(Flamethrower), nameof(Missiles), nameof(Artillery), nameof(Troops) };
    private Queue<string> attackPattern = new();
    private HPComponent hp;
    private Transform playerLoc;
    private Rigidbody2D rb;
    private bool turretattack;
    private Animator animator;

    // animations

    const string TURRETS_EXTRACT = "TurretExtract";
    const string TURRETS_RETRACT = "TurretRetract";


    private void Start()
    {
        hp = GetComponent<HPComponent>();
        if (hp) hp.OnHPZero = OnDied;
        playerLoc = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        NextAttack();
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
        if (!CanUseAttack(tso.turrets_maxHpForUse))
        {
            NextAttack();
            yield break;
        }

        // expose
        foreach (var item in turrets)
        {
            animator.Play(TURRETS_EXTRACT);
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
                    Debug.Log(rotdone);
                }

                if (Mathf.Abs(angle) < 1f && !retract)
                {
                    retract = true;
                    animator.Play(TURRETS_RETRACT);
                }
                
            }
            yield return null;
        }


        yield return new WaitForSeconds(tso.turrets_delayBeforeNextAttack);
        NextAttack();
    }

    private IEnumerator Flamethrower()
    {
        if (!CanUseAttack(tso.flamethrower_maxHpForUse))
        {
            NextAttack();
            yield break;
        }

        // this line is only here because otherwise it will give an error
        // when you start coding, move it to where you need it
        yield return null;
        NextAttack();
    }

    private IEnumerator Missiles()
    {
        if (!CanUseAttack(tso.missile_maxHpForUse))
        {
            NextAttack();
            yield break;
        }

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

        yield return new WaitForSeconds(tso.missile_delayBeforeNextAttack);
        NextAttack();
    }

    private IEnumerator Artillery()
    {
        if (!CanUseAttack(tso.Artillery_maxHpForUse))
        {
            NextAttack();
            yield break;
        }

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
