using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Ranged",menuName = "Enemy/Ranged")]
public class RangedScriptableObject : EnemyScriptableObject
{
    [Header("Ranged"), Space(5f)]
    public float damage;
    public float delayBetweenShots = 0.5f;
    public float shootStartDelay = 0.3f;
    public float bulletSpeed = 15f;
    public float bulletSpread = 10f;
    public GameObject bullet;
    public Transform bulletSpawn;
    public GameObject muzzleFlash;
}
