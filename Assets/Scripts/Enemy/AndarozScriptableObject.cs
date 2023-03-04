using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Base",menuName = "Enemy/Andaroz")]
public class AndarozScriptableObject : EnemyScriptableObject
{
    [Space(10f)]
    [Header("Melee Attack 1 - Swipe"), Space(5f)]
    public float swipe_damage;
    public float swipe_angle = 180f;
    public float swipe_cooldown = 1f;
    public float swipe_applyDmgDelay = 0.5f;
    public float swipe_delayBeforeNextAttack = 3f;
    public float swipe_delayBeforeIdle = 1f;
    public float swipe_maxAttackTime = 10f;
    public float swipe_maxAttackDistance = 5f;
    public float swipe_minAttackDistance = 4f;
    public float swipe_comfortableAttackDist = 0f;
    public float swipe_attackMovementSpeed = 8f;
    public float swipe_runSpeed = 8f;
    public float swipe_retreatSpeed = 0f;
    public bool swipe_chasePlayer = true;
    public bool swipe_lockRotation = true;
    public float swipe_lockRotDelay = 0.2f;
    //public bool swipe_damagePillars = true;
    [Range(0f, 1f)] public float swipe_maxHpForUse = 1f;

    [Space(10f)]
    [Header("Melee Attack 2 - AOE Ground Slam"), Space(5f)]
    public float slam_damage;
    public float slam_cooldown = 1f;
    public float slam_applyDmgDelay = 0.5f;
    public float slam_reach;
    public float slam_delayBeforeNextAttack = 3f;
    public float slam_delayBeforeIdle = 1f;
    public float slam_maxAttackTime = 10f;
    public float slam_maxAttackDistance = 5f;
    public float slam_minAttackDistance = 4f;
    public float slam_comfortableAttackDist = 0f;
    public float slam_attackMovementSpeed = 8f;
    public float slam_runSpeed = 8f;
    public float slam_retreatSpeed = 0f;
    public bool slam_chasePlayer = true;
    public bool slam_lockRotation = true;
    public float slam_lockRotDelay = 0.2f;
    public GameObject slam_Particles;
    //public bool slam_damagePillars = true;
    [Range(0f, 1f)] public float slam_maxHpForUse = 1f;

    [Space(10f)]
    [Header("Ranged Attack 1 - Machine Gun"), Space(5f)]
    public float gun_damage;
    public float gun_delayBetweenShots = 0.5f;
    public float gun_shootStartDelay = 0.3f;
    public float gun_delayBeforeNextAttack = 3f;
    public float gun_delayBeforeIdle = 1f;
    public float bulletSpeed = 15f;
    public float bulletSpread = 10f;
    public int shots_bullets = 50;
    public GameObject bullet;
    public float gun_minAttackDistance = 10f;
    public float gun_maxAttackDistance = 15;
    public float gun_attackMovementSpeed = 3f;
    public float gun_comfortableAttackDist = 0f;
    public float gun_runSpeed = 3f;
    public float gun_retreatSpeed = 0f;
    public bool gun_chasePlayer = true;
    //public bool gun_damagePillars = false;
    [Range(0f, 1f)] public float gun_maxHpForUse = 1f;

    [Space(10f)]
    [Header("Ranged Attack 2 - Missile Barrage"), Space(5f)]
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
    //public bool missile_damagePillars = true;
    [Range(0f, 1f)] public float missile_maxHpForUse = 0.9f;

    [Space(10f)]
    [Header("Ranged Attack 3 - Twin Laser"), Space(5f)]
    public float laser_damage;
    public float delayBeforeParticles = 1f;
    public float delayParticlesToLaser = 1.5f;
    public int laser_durationFrames = 93;
    public float delayLaserEndToIdle = 1f;
    public float laser_delayBeforeNextAttack = 1f;
    public float laser_maxAttackDistance = 40f;
    public float laser_minAttackDistance = 30f;
    public float laser_comfortableAttackDist = 0f;
    public float laser_runSpeed = 0f;
    public float laser_retreatSpeed = 0f;
    public float laser_attackMovementSpeed = 0f;
    public bool laser_chasePlayer = false;
    //public bool laser_damagePillars = true;
    [Range(0f, 1f)] public float laser_maxHpForUse = 0.6f;

    [Space(10f)]
    [Header("Stage 2"), Space(5f)]
    [Range(0f, 1f)] public float HpForStage2 = 0.5f;
    public GameObject deadMe;

    [Space(10f)]
    [Header("Testing And Debugging"), Space(5f)]
    public bool enableSwipe = true;
    public bool enableSlam = true;
    public bool enableMachineGun = true;
    public bool enableMissiles = true;
    public bool enableLaser = true;
}
