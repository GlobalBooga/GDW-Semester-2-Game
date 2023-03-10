using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Base",menuName = "Enemy/Base")]
public class EnemyScriptableObject : ScriptableObject
{
    [Header("Detection"), Space(5f)]
    public float defaultReactionTime = 0f;
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
    public float defualtWalkSpeed = 3f;
    public float defaultRunSpeed = 8f;
    public float defaultAttackMovementSpeed = 3f;
    public float defaultRetreatSpeed = 3f;
    public float defaultMoveForce = 15f;
    public float defaultAccelerationDrag = 1f;
    public float defaultDeccelerationDrag = 5f;
    public float snappyness = 0.5f;
    public AnimationCurve test;

    [Space(10f)]
    [Header("Aggressive Behaviour"), Space(5f)]
    public float defaultMaxAttackDistance = 10f;
    public float defaultMinAttackDistance = 6f;
    public float defaultComfortableAttackDist;
    public bool chasePlayer = true;

    [Space(10f)]
    [Header("Masks"), Space(5f)]
    public LayerMask whatIsPlayer;
    public LayerMask whatIsWall;
    public LayerMask whatBlocksSight;
    public LayerMask whatTakesDamage;
}
