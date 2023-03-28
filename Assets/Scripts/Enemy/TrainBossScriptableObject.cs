using System;
using UnityEngine;

[CreateAssetMenu(fileName ="Base",menuName = "Enemy/TrainBoss")]
public class TrainBossScriptableObject : ScriptableObject
{

    [Space(10f)]
    [Header("Attack 1 - Turrets"), Space(5f)]
    public GameObject bullet;
    public float turrets_damage;
    public float turrets_delayBetweenShots = 0.5f;
    public float turrets_aimStartDelay = 0.3f;
    public float turrets_shootStartDelay = 0.3f;
    public float turrets_delayBeforeNextAttack = 3f;
    public float bulletSpeed = 15f;
    public float bulletSpread = 10f;
    public int shots_Turrets = 50;
    public float rotationSpeed = 1f;
    public float maxAngle = 90f;
    public float extrudeDistance = 2f;
    public float turrets_coverDamageMultiplier = 0;
    [Range(1, 7)] public int turrets_maxHpForUse = 1;
    [Space(10f)]

    [Header("Attack 2 - Flamethrower"), Space(5f)]
    public bool flamethrower_useWithOtherAttacks;
    public float flamethrower_damage;
    public float flamethrower_dmgInterval;
    public float flamethrower_shootStartDelay = 0.3f;
    public float flamethrower_timeBeforeRetract = 1.5f;
    public float flamethrower_delayBeforeNextAttack = 6f;
    public int flamethrower_timeBeforeEnd = 5;
    public bool aimForPlayer;
    public float maxXmove;
    public float moveSpeed;
    public float flamethrower_coverDamageMultiplier = 0;
    [Range(1, 7)] public int flamethrower_maxHpForUse = 2;

    [Space(10f)]
    [Header("Attack 3 - Missile Barrage"), Space(5f)]
    public GameObject crosshairController;
    public GameObject missile;
    public float missile_damage;
    public float missile_delayBetweenShots = 0.5f;
    public float missile_aimStartDelay = 0.3f;
    public float missile_shootStartDelay = 0.3f;
    public float missile_delaybeforeRetract = 3f;
    public float missile_delayBeforeNextAttack = 3f;
    public float missileRotForce = 10f;
    public float missileMaxSpeed = 7f;
    public float missileSpread = 10f;
    public int shots_missiles = 8;
    public LayerMask whatTakesDamageFromMissiles;
    public float missile_coverDamageMultiplier = 0.5f;
    [Range(1, 7)] public int missile_maxHpForUse = 1;

    [Space(10f)]
    [Header("Attack 4 - Artillery Barrage"), Space(5f)]
    public bool artillery_useWithOtherAttacks;
    public GameObject artilleryStrike;
    public float artillery_damage;
    public float artillery_accuracy = 0.3f;
    public float artillery_imactDelay = 2f;
    public float artillery_cameraShakeIntensity = 6f;
    public float artillery_cameraShakeTime = 0.2f;
    public float artillery_delayBetweenShots = 1f;
    public float artillery_airTime = 3f;
    public float artillery_shootStartDelay = 0.3f;
    public float artillery_delayBeforeNextAttack = 6f;
    public int artillery_shots = 3;
    //public bool fireSequentiallyArtillery = true;
    public float artillery_pillarDamageMultiplier = 0.75f;
    [Range(1, 7)] public int Artillery_maxHpForUse = 1;

    [Space(10f)]
    [Header("Attack 5 - Troop Deploy"), Space(5f)]
    public float troop_startDelay = 10f;
    public float troop_delayBeforeNextAttack = 6f;
    public GameObject[] troop_types;
    public float troop_spawnAmount = 5;
    [Range(1, 7)] public int troopDeploy_maxHpForUse = 3;

    [Space(10f)]
    [Header("Debugging"), Space(5f)]
    public bool enableTurrets;
    public bool enableFlamethrower;
    public bool enableMissiles;
    public bool enableArtillery;
    public bool enableTroops;


    [Serializable]
    public struct MultiBarrelMissileLauncher
    {
        public bool fireSequentially;
        public Transform mainUnit;
        public Transform[] missileSpawns;

        public void FireAt(Transform target, GameObject missile, float damage, float spread, float speed, float rotForce, float coverDmgMult, LayerMask whatTakesDamage, ref int barrelIndex)
        {
            // calculate spread
            float spreadAngle = UnityEngine.Random.Range(-spread, spread);
            float rads = Mathf.Deg2Rad * ((spreadAngle > 0) ? spreadAngle : (360f + spreadAngle));

            Vector3 targetDir = (target.position - mainUnit.position).normalized;

            float x = targetDir.x, y = targetDir.y;

            Vector3 missileDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));

            HomingMissile m;

            // making sure there are no null refs (ish)
            if (missile && missileSpawns.Length > 0)
            {
                if (missileSpawns.Length == 1)
                {
                    m = Instantiate(missile, missileSpawns[0]).GetComponent<HomingMissile>();
                }
                else if (!fireSequentially)
                {
                    m = Instantiate(missile, missileSpawns[UnityEngine.Random.Range(0, missileSpawns.Length)]).GetComponent<HomingMissile>();
                }
                else
                {
                    if (barrelIndex >= missileSpawns.Length) barrelIndex = 0;
                    m = Instantiate(missile, missileSpawns[barrelIndex++]).GetComponent<HomingMissile>();
                }

                m.transform.Rotate(0f, 0f, Vector2.SignedAngle(targetDir, missileDir));
                m.gameObject.layer = StaticHelpers.EnemyMissile;
                m.Fly(target, missileDir, rotForce, speed, damage);
                m.specialObjectDamageMultiplier = coverDmgMult;
                m.explosive.SetWhatTakesDamage(whatTakesDamage);
            }
            
        }
    }

    [Serializable]
    public struct MachineGunTurret
    {
        public bool enabled;
        public Transform mainUnit;
        public Transform bulletSpawn;
        public GameObject muzzleFlash;

        public void Fire(GameObject bullet, float damage, float spread, float speed, float coverDmgMult = 0)
        {
            if (!enabled) return;

            // calculate spread
            float spreadAngle = UnityEngine.Random.Range(-spread, spread);
            float rads = Mathf.Deg2Rad * ((spreadAngle > 0) ? spreadAngle : (360f + spreadAngle));
            float x = -mainUnit.right.x, y = -mainUnit.right.y;

            Vector3 bulletDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));

            if (bullet && bulletSpawn)
            {
                Bullet b = Instantiate(bullet, bulletSpawn).GetComponent<Bullet>();
                b.transform.Rotate(0f, 0f, Vector2.SignedAngle(-mainUnit.right, bulletDir));
                b.gameObject.layer = StaticHelpers.EnemyProjectileLayer;
                b.Fly(bulletDir, speed, damage);
                b.specialObjectDamageMultiplier = coverDmgMult;

                //play muzzle effect
                if (muzzleFlash) muzzleFlash.SetActive(true);
            }
        }
    }

    [Serializable]
    public struct Flamethrower
    {
        public GameObject flameZone;
        public Transform mainUnit;
        public BoxCollider2D fireZone;
    }
}

