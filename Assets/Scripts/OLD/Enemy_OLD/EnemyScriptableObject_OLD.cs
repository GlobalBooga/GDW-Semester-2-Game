using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Enemies/Dummy", fileName ="New Dummy")]
public class EnemyScriptableObject : ScriptableObject
{
    [Header("Aggressive Behaviour"), Space(5f)]
    public float attackDamage;
    public float attackCooldown;
    public float attackDelay;
    public float attackRange;
    public float attackArcAngle;
    private bool attackReady;
    [Space(10f)]

    [Header("Detection"), Space(5f)]
    public float reactionTime;
    public float viewDistance;
    public float viewArc;
    public float maxSearchTime;
    [Space(10f)]

    [Header("Search"), Space(5f)]
    public float maxWaitTime = 1f;
    public float minWaitTime = 3f;
    [Space(10f)]

    [Header("Movement"), Space(5f)]
    public float moveSpeed = 1f;
    public float walkSpeed = 1f;
    public float chaseSpeed;
    public float accelerationDrag = 1f;
    public float deccelerationDrag = 5f;
}
