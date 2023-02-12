using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndarozScriptableObject : MonoBehaviour
{
    [Header("Melee Attack 1 - Swipe"), Space(5f)]
    public float swipeDamage;
    public float swipeCooldown = 1f;
    public float swipeApplyDmgDelay = 0.5f;
    public float swipeReach;
    public float delayBeforeNextAttack_swipe = 3f;
    public float maxAttackTime_swipe = 10f;
    [Range(0f, 1f)] public float maxHpForUse_swipe = 1f;

    [Space(10f)]
    [Header("Melee Attack 2 - AOE Ground Slam"), Space(5f)]
    public float slamDamage;
    public float slamCooldown = 1f;
    public float slamApplyDmgDelay = 0.5f;
    public float slamReach;
    public float delayBeforeNextAttack_slam = 3f;
    public float maxAttackTime_slam = 10f;
    [Range(0f, 1f)] public float maxHpForUse_slam = 1f;

    [Space(10f)]

    [Header("Ranged Attack 1 - Machine Gun"), Space(5f)]
    public float gunDamage;
    public float delayBetweenShots_gun = 0.5f;
    public float shootStartDelay_gun = 0.3f;
    public float delayBeforeNextAttack_gun = 3f;
    public float bulletSpeed = 15f;
    public float bulletSpread = 10f;
    public int shots_bullets = 50;
    public GameObject bullet;
    [Range(0f, 1f)] public float maxHpForUse_gun = 1f;

    [Space(10f)]

    [Header("Ranged Attack 2 - Missile Barrage"), Space(5f)]
    public float missileDamage;
    public float delayBetweenShots_missiles = 0.5f;
    public float shootStartDelay_missiles = 0.3f;
    public float delayBeforeNextAttack_missiles = 6f;
    public float missileSpeed = 7f;
    public float missileRotForce = 10f;
    public float missileMaxSpeed = 7f;
    public float missileSpread = 10f;
    public int shots_missiles = 10;
    public GameObject crosshairController;
    public GameObject missile;
    public bool fireSequentially = true;
    [Range(0f, 1f)] public float maxHpForUse_missiles = 0.9f;

    [Space(10f)]

    [Header("Ranged Attack 3 - Twin Laser"), Space(5f)]
    public float laserDamage;
    public float attackDuration;
    public float delayBeforeNextAttack_laser = 3f;
    public float rotationSpeed;
    public float maxDist = 100f;
    
    [Range(0f, 1f)] public float maxHpForUse_laser = 0.6f;

    [Space(10f)]

    [Header("Attack Pattern"), Space(5f)]
    public List<string> orderedAttacks;
    [Range(0f, 1f)] public float HpForStage2 = 0.5f;

    [Space(10f)]

    [Header("Testing And Debugging"), Space(5f)]
    public bool enableSwipe = true;
    public bool enableSlam = true;
    public bool enableMachineGun = true;
    public bool enableMissiles = true;
    public bool enableLaser = true;
}
