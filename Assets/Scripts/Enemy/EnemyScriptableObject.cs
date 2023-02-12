using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Base",menuName = "Enemy/Base")]
public class EnemyScriptableObject : ScriptableObject
{
    [Header("Detection"), Space(5f)]
    public float reactionTime = 0f;
    public float viewDistance = 40f;
    public float instantDetectDist = 3f;
    public float normalFOV = 60f;
    public float searchFOV = 120f;
    public AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public int maxStoredPoses = 4;
    public float secondsBetweenPoses = 1f;
    public float minDistanceBetweenPoses = 1f;
    public float searchDetectionRateMult = 1f;
    public bool canSeeThroughWalls;

    [Space(10f)]

    [Header("Movement"), Space(5f)]
    public float walkSpeed = 3f;
    public float runSpeed = 8f;
    public float attackMovementSpeed = 3f;
    public float retreatSpeed = 3f;
    public float moveForce = 15f;
    public float accelerationDrag = 1f;
    public float deccelerationDrag = 5f;
    [Space(10f)]

    [Header("Aggressive Behaviour"), Space(5f)]
    public float maxAttackDistance = 10f;
    public float minAttackDistance = 6f;
    public float comfortableAttackDist;
    public bool chasePlayer = true;


    [Space(10f)]

    [Header("Masks"), Space(5f)]
    public LayerMask whatIsPlayer;
    public LayerMask whatIsWall;
    public LayerMask whatBlocksSight;
    public LayerMask whatTakesDamage;
}
