using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class HomingArrow : HomingMissile
{
    internal override void Awake()
    {
        base.Awake();
    }

    public override void Fly(Transform target, Vector2 initialDir, float rotationForce, float maxSpeed, float damage = 0)
    {
        base.Fly(target, initialDir, rotationForce, maxSpeed, damage);
    }

    internal override void Update()
    {
        if (target)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, Vector3.SignedAngle(rb.velocity, Vector3.up, Vector3.back)); ;
        }
    }

    internal override void FixedUpdate()
    {
        if (target) base.FixedUpdate();
    }

    internal override void OnCollisionEnter2D(Collision2D collision)
    {
        StaticHelpers.ApplyDamage(collision.gameObject, damage);
        Destroy(gameObject);
    }
}
