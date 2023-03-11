using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.Networking.Types;

public class TankBoss : MonoBehaviour
{
    public List<Transform> turretSpawns;
    public List<Transform> missileSpawns;
    public List<Transform> flameThrowerSpawn;
    public Transform artillarySpawn;
    public List<Transform> troopSpawn;

    public GameObject muzzleFlash;
    private TankBossScriptableObject tso;

    private int prev; // the previous index for sequential firing
    private List<string> allAttacks = new() { nameof(Turrets), nameof(FlameThrower), nameof(Missiles), nameof(Artillary), nameof(Troops) };
    private Queue<string> attackPattern = new();

    [SerializeField] private bool enableTurrets;
    [SerializeField] private bool enableFlameThrower;
    [SerializeField] private bool enableMissles;
    [SerializeField] private bool enableArtillary;
    [SerializeField] private bool enableTroops;

    public Animator TankAnimator;
    public GameObject deathExplosive;
    public List<Transform> deathExplosionTransforms;
    public GameObject[] enemies; 
    private bool goToCenterOfRoom;
    private Vector3 centerOfRoom;
    private Transform playerLoc;
    private Rigidbody2D rb;
    private bool fight = false; 

    public float maxXPos;
    public float maxYPos; 
    public float minXPos; 
    public float minYPos; 
    // ANIMATION KEYWORDS

    private const string DEATH = "Tank_Death";

    HPComponent hp;

    private void Start()
    {
        hp = GetComponent<HPComponent>();
        if (hp) hp.OnHPZero = OnDied;
        playerLoc = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>(); 
    }

    private void Update()
    {

    }
    private void Attack()
    {
        NextAttack();
    }
    private IEnumerator Turrets()
    {
        {
            yield return new WaitForSeconds(tso.Turrets_shootStartDelay);

            // alert interval
            float timeInterval = 1f;
            float time = Time.time - timeInterval;

            // alert everyone
            if (Time.time - time >= timeInterval)
            {
                time = Time.time;
                LevelManager.instance.AlertAllEnemiesInCurrentScene(transform.position);
            }

            // calculate spread
            float spreadAngle = Random.Range(-tso.bulletSpread, tso.bulletSpread);
            float rads = Mathf.Deg2Rad * ((spreadAngle > 0) ? spreadAngle : (360f + spreadAngle));
            float x = transform.up.x, y = transform.up.y;

            Vector3 bulletDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));

            // muzzleFlash
            if (tso.muzzleFlash) tso.muzzleFlash.SetActive(true);    

            if (tso.bullet && turretSpawns[0])
            {
                Bullet b = Instantiate(tso.bullet, turretSpawns[0]).GetComponent<Bullet>();
                b.transform.Rotate(0f, 0f, Vector2.SignedAngle(transform.up, bulletDir));
                b.gameObject.layer = StaticHelpers.EnemyProjectileLayer;
                b.Fly(bulletDir, tso.bulletSpeed, tso.Turrets_damage);

                //play muzzle effect
            }

            yield return new WaitForSeconds(tso.Turrets_delayBetweenShots);
        }
    }
    
    private IEnumerator FlameThrower()
    {
        // this line is only here because otherwise it will give an error
        // when you start coding, move it to where you need it
        yield return null;
    }

    private IEnumerator Missiles()
    {
        int shots = 0;
        prev = 0;

        while (shots++ < tso.shots_missiles)
        {
            // calculate spread
            float spreadAngle = Random.Range(-tso.missileSpread, tso.missileSpread);
            float rads = Mathf.Deg2Rad * ((spreadAngle > 0) ? spreadAngle : (360f + spreadAngle));
            float x = transform.up.x, y = transform.up.y;

            Vector3 missileDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));

            if (tso.missile && missileSpawns.Count > 0)
            {
                HomingMissile b;
                if (missileSpawns.Count == 1)
                {
                    b = Instantiate(tso.missile, missileSpawns[0]).GetComponent<HomingMissile>();
                }
                else if (!tso.fireSequentially)
                {
                    b = Instantiate(tso.missile, missileSpawns[Random.Range(0, missileSpawns.Count)]).GetComponent<HomingMissile>();
                }
                else
                {
                    if (prev >= missileSpawns.Count) prev = 0;
                    b = Instantiate(tso.missile, missileSpawns[prev++]).GetComponent<HomingMissile>();
                }

                b.transform.Rotate(0f, 0f, Vector2.SignedAngle(transform.up, missileDir));
                b.gameObject.layer = StaticHelpers.EnemyMissile;
                b.Fly(playerLoc, missileDir, tso.missileRotForce, tso.missileMaxSpeed, tso.missile_damage);
                b.specialObjectDamageMultiplier = tso.missile_pillarDamageMultiplier;
                //play muzzle effect
            }
            yield return new WaitForSeconds(tso.missile_delayBetweenShots);
        }
    }

    private IEnumerator Artillary()
    {
        yield return null;
    }
    
    private IEnumerator Troops()
    {
        int RandomSpawnNumber = Random.Range(1, 3); 
        if(RandomSpawnNumber == 1)
        {
            InvokeRepeating("TroopSpawnBottom", 1f, 20f);
        }
        if(RandomSpawnNumber == 2)
        {
            InvokeRepeating("TroopSpawnSide", 1f, 20f);
        }
        
        yield return new WaitForSeconds(tso.Troop_deployBeforeNextAttack);
    }

    private void TroopSpawnBottom()
    {
        float RandomX = Random.Range(minXPos, maxXPos);
        transform.position = new Vector3(RandomX, transform.position.y, transform.position.z);
        int RandomEnemy = Random.Range(0, enemies.Length);

        Instantiate(enemies[RandomEnemy], transform.position, Quaternion.identity);
    }

    private void TroopSpawnSide()
    {
        float RandomY = Random.Range(minYPos, maxYPos);
        transform.position = new Vector3(transform.position.x, RandomY, transform.position.z);
        int RandomEnemy = Random.Range(0, enemies.Length);

        Instantiate(enemies[RandomEnemy], transform.position, Quaternion.identity);
    }
    /// <summary>
    /// I copied this from the Andaroz script.
    /// It will make a random list of attacks and store it in the 'attackPattern' queue.
    /// </summary>
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
    /// <summary>
    /// I copied this from the Andaroz script.
    /// It will check if the next attack is usable 
    /// reason = is the boss's hp low enough to use this attack? 
    /// Is this attack enabled (for debugging))
    /// </summary>
    private void NextAttack()
    {
        if (!enableTurrets && !enableFlameThrower && !enableMissles && !enableArtillary && !enableTroops)
        {
            Debug.LogWarning("No attacks enabled! Enabling attack 1");
            enableTurrets = true;
        }

        if (attackPattern.Count == 0) NewAttackOrder();

        // skip disabled attacks
        if (attackPattern.Peek() == nameof(Turrets) && enableTurrets) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(FlameThrower) && enableFlameThrower) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(Missiles) && enableMissles) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(Artillary) && enableArtillary) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(Troops) && enableTroops) { attackPattern.Dequeue(); NextAttack(); return; }

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
