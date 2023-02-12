using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Sniper",menuName = "Enemy/Sniper")]
public class SniperScriptableObject : EnemyScriptableObject
{
    [Header("Sniper"), Space(5f)]
    public float damage;
    public float delayBetweenShots = 1f;
    public float aimStartDelay = 0.5f;
    public float aimTime = 3f;
    public int warningFlashes = 3;
    public float laserFlashOnTime = 0.25f;
    public float laserFlashOffTime = 0.25f;

    [Space(10)]

    [Header("Laser"), Space(5f)]
    public float maxDist = 100f;
    public Transform laserStart;
    public LineRenderer lineRenderer;
    public Animator animator;
    public float animationLenght = 0.1f;
}
