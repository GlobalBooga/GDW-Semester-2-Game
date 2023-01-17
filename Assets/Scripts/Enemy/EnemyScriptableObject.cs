using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Enemies/Dummy", fileName ="New Dummy")]
public class EnemyScriptableObject : ScriptableObject
{
    [Header("Health"), Space(5f)]
    public float maxHealth;
    float health;
    [Space(10f)]

    [Header("Attack"), Space(5f)]
    public float attackDamage;
    public float attackRate;
    public float attackRange;
    public float attackArcAngle;
    [Space(10f)]

    [Header("Detection"), Space(5f)]
    public float reactionTime;
    public float viewDistance;
    public float viewArc;
    public float maxSearchTime;
    public LayerMask whatIsPlayer;



    public virtual void TakeDamage(float amount)
    {
        health = Mathf.Clamp(health - amount, 0f, maxHealth);
    }
}
