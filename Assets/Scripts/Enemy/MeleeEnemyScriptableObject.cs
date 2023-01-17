using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Melee", fileName = "New Melee Enemy")]
public class MeleeEnemyScriptableObject : EnemyScriptableObject
{
    [Space(10f)]
    [Header("Movement"), Space(5f)]
    public float walkSpeed;
    public float chaseSpeed;

}
