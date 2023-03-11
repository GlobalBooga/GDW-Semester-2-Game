using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBossScriptableObject : EnemyScriptableObject

{
    [Space(10f)]
    [Header("Ranged Attack 1 - Turrets"), Space(5f)]
    public float Turrets_damage;
    public float Turrets_delayBetweenShots = 0.5f;
    public float Turrets_shootStartDelay = 0.3f;
    public float Turrets_delayBeforeNextAttack = 3f;
    public float Turrets_delayBeforeIdle = 1f;
    public float bulletSpeed = 15f;
    public float bulletSpread = 10f;
    public int shots_Turrets = 50;
    public GameObject bullet;
    public float Turrets_minAttackDistance = 10f;
    public float Turrets_maxAttackDistance = 15;
    public float Turrets_attackMovementSpeed = 3f;
    public float Turrets_comfortableAttackDist = 0f;
    public float Turrets_runSpeed = 3f;
    public float Turrets_retreatSpeed = 0f;
    public bool Turrets_chasePlayer = true;
    public float Turrets_pillarDamageMultiplier = 1;
    public GameObject muzzleFlash;
    [Range(0f, 1f)] public float Turrets_maxHpForUse = 1f;
    [Space(10f)]

    [Header("Ranged Attack 2 - FlameThrower"), Space(5f)]
    public float flameThrower_damage;
    public float flameThrower_delayBetweenShots = 0.5f;
    public float flameThrower_shootStartDelay = 0.3f;
    public float flameThrower_delayBeforeNextAttack = 6f;
    public float flameThrower_delayBeforeIdle = 1f;
    public float flameThrowerSpeed = 7f;
    public float flameThrowerRotForce = 10f;
    public float flameThrowerMaxSpeed = 7f;
    public float flameThrowerSpread = 10f;
    public int shots_flameThrower = 60;
    public GameObject flameThrower;
    public float flameThrower_maxAttackDistance = 40f;
    public float flameThrower_minAttackDistance = 30f;
    public float flameThrower_comfortableAttackDist = 0f;
    public float flameThrower_runSpeed = 0f;
    public float flameThrower_retreatSpeed = 0f;
    public float flameThrower_attackMovementSpeed = 0f;
    public bool flameThrower_chasePlayer = false;
    public bool flameThrower_lockRotation = true;
    public float flameThrower_lockRotDelay = 0.2f;
    public float flameThrower_pillarDamageMultiplier = 1;
    [Range(0f, 1f)] public float flameThrower_maxHpForUse = 0.9f;

    [Space(10f)]
    [Header("Ranged Attack 3 - Missile Barrage"), Space(5f)]
    public float missile_damage;
    public float missile_delayBetweenShots = 0.5f;
    public float missile_shootStartDelay = 0.3f;
    public float missile_delayBeforeNextAttack = 6f;
    public float missile_delayBeforeIdle = 1f;
    public float missileSpeed = 7f;
    public float missileRotForce = 10f;
    public float missileMaxSpeed = 7f;
    public float missileSpread = 10f;
    public int shots_missiles = 10;
    public GameObject crosshairController;
    public GameObject missile;
    public bool fireSequentially = true;
    public float missile_maxAttackDistance = 40f;
    public float missile_minAttackDistance = 30f;
    public float missile_comfortableAttackDist = 0f;
    public float missile_runSpeed = 0f;
    public float missile_retreatSpeed = 0f;
    public float missile_attackMovementSpeed = 0f;
    public bool missile_chasePlayer = false;
    public bool missile_lockRotation = true;
    public float missile_lockRotDelay = 0.2f;
    public float missile_pillarDamageMultiplier = 1;
    [Range(0f, 1f)] public float missile_maxHpForUse = 0.9f;

    [Space(10f)]
    [Header("Ranged Attack 4 - Artillery Barrage"), Space(5f)]
    public float Artillery_damage;
    public float Artillery_delayBetweenShots = 1f;
    public float Artillery_shootStartDelay = 0.3f;
    public float Artillery_delayBeforeNextAttack = 6f;
    public float Artillery_delayBeforeIdle = 1f;
    public float ArtillerySpeed = 7f;
    public float ArtilleryRotForce = 10f;
    public float ArtilleryMaxSpeed = 7f;
    public float ArtillerySpread = 10f;
    public int shots_Artillery = 10;
    public GameObject Artillery;
    public bool fireSequentiallyArtillery = true;
    public float Artillery_maxAttackDistance = 40f;
    public float Artillery_minAttackDistance = 30f;
    public float Artillery_comfortableAttackDist = 0f;
    public float Artillery_runSpeed = 0f;
    public float Artillery_retreatSpeed = 0f;
    public float Artillery_attackMovementSpeed = 0f;
    public bool Artillery_chasePlayer = false;
    public bool Artillery_lockRotation = true;
    public float Artillery_lockRotDelay = 0.2f;
    public float Artillery_pillarDamageMultiplier = 1;
    [Range(0f, 1f)] public float Artillery_maxHpForUse = 0.9f;

    [Space(10f)]
    [Header("Attack 5 - Troop Deploy"), Space(5f)]
    public float Troop_deployDelay = 10f;
    public float Troop_deployBeforeNextAttack = 6f;
    public float Troop_deplyBeforeIdle = 1f; 
    public GameObject Troop;
    public float deployXValue;
    public float deplyYValue;
    [Range(0f, 1f)] public float troopDeploy_maxHpForUse = 0.9f;

    [Space(10f)]
    [Header("Stage 2"), Space(5f)]
    [Range(0f, 1f)] public float HpForStage2 = 0.5f;
    public GameObject deadMe;

    [Space(10f)]
    [Header("Debugging"), Space(5f)]
    public bool enableTurrets;
    public bool enableFlameThrower;
    public bool enableMissles;
    public bool enableArtillary;
    public bool enableTroops;
}

