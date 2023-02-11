using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public Animator animator;
    private const string anim = "MissileLockOn";
    public float animationTime = 1.65f;
    private Transform target;
    private float oldz;

    private void Update()
    {
        if (target) transform.position = target.position;
    }

    private void SetRandomRotation()
    {
        float newz = Random.Range(0, 4);
        while (newz == oldz) newz = Random.Range(0, 4);

        oldz = newz;
        transform.rotation = Quaternion.Euler(0f, 0f, newz * 90f);
    }

    public void AimAt(Transform target)
    {
        this.target = target;
        SetRandomRotation();
        if (animator) animator.Play(anim);
    }
    
}
