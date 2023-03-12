using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TrainBossScriptableObject;

public class TrainBoss : MonoBehaviour
{
    public TrainBossScriptableObject tso;
    public List<MultiBarrelMissileLauncher> missileLaunchers;


    private int prev; // the previous index for sequential firing
    private List<string> allAttacks = new() { nameof(Turrets), nameof(FlameThrower), nameof(Missiles), nameof(Artillery), nameof(Troops) };
    private Queue<string> attackPattern = new();
    private HPComponent hp;
    private Transform playerLoc;
    private Rigidbody2D rb;

    private void Start()
    {
        hp = GetComponent<HPComponent>();
        if (hp) hp.OnHPZero = OnDied;
        playerLoc = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        Attack();
    }

    private void Attack()
    {
        NextAttack();
    }
    private IEnumerator Turrets()
    {
        yield return null;
        NextAttack();
    }

    private IEnumerator FlameThrower()
    {
        // this line is only here because otherwise it will give an error
        // when you start coding, move it to where you need it
        yield return null;
        NextAttack();
    }

    private IEnumerator Missiles()
    {
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
        if (attackPattern.Peek() == nameof(FlameThrower) && !tso.enableFlamethrower) { attackPattern.Dequeue(); NextAttack(); return; }
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

}
