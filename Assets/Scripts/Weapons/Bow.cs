using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Gun
{
    [Header("Bow Properties"), Space(5f)]
    public float lockOnDistance = 20f;
    public float lockOnAngle = 60f;
    public float homingForce = 10f;
    public LayerMask whatIsEnemy;

    internal override void Awake()
    {
        base.Awake();
    }

    internal override void OnValidate()
    {
        base.OnValidate();
    }

    internal override void ResetUse()
    {
        base.ResetUse();
    }

    public override void Use()
    {
        if (!readyToUse) return;
        readyToUse = false;

        // custom bow use behaviour since it is not a gun

        Transform target = null;

        //lockon to a target
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, lockOnDistance, whatIsEnemy);
        foreach (var item in col)
        {
            // check if we are facing this enemy
            if (Vector3.Angle(transform.up, item.transform.position - transform.position) <= lockOnAngle * 0.5f)
            {
                // are they visible?
                //RaycastHit2D hit = Physics2D.Raycast(transform.position, (item.transform.position - transform.position).normalized, lockOnDistance, whatIsEnemy);
                //if (hit)
                //{
                //}
                //Debug.Log(hit.collider.gameObject.name);
                //Debug.DrawLine(transform.position, hit.transform.position, Color.green, 2f);
                target = item.transform;
            }
        }

        if (bullet && bulletSpawn)
        {
            HomingArrow a = Instantiate(bullet, bulletSpawn).GetComponent<HomingArrow>();
            a.gameObject.layer = StaticHelpers.PlayerProjectileLayer;
            if (target) a.Fly(target, transform.parent.up, homingForce, bulletSpeed, damage);
            else a.Fly(transform.up, bulletSpeed, damage);
        }

        if (cooldown > 0) Invoke(nameof(ResetUse), cooldown);
        else ResetUse();
    }

    public override void Drop(Vector2 forwards)
    {
        base.Drop(forwards);
    }

    public override void Pickup(Transform parentTo, Weapon weapon)
    {
        base.Pickup(parentTo, weapon);
    }

    public override void UseAbility()
    {

    }

    public override void EndAbility()
    {
    
    }

    
}
