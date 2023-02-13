using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Melee",menuName = "Enemy/Melee")]
public class MeleeScriptableObject : EnemyScriptableObject
{
    [Header("Melee"), Space(5f)]
    public float damage;
    public float attackCooldown = 1f;
    public float applyDmgDelay = 0.5f;
}
