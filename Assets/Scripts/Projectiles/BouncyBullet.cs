using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyBullet : Bullet
{
    const int maxBounces = 3;
    int bounces;

    Vector2 lastVelocity;

    internal override void Awake()
    {
        base.Awake();
        Destroy(gameObject, 15f);
    }

    private void Update()
    {
        lastVelocity = rb.velocity;
    }

    internal override void OnCollisionEnter2D(Collision2D collision)
    {
        if (StaticHelpers.ApplyDamage(collision.gameObject, damage))
        {
            Destroy(gameObject);
        }

        if (bounces < maxBounces)
        {
            bounces++;
            Vector2 d = lastVelocity.normalized;
            Vector2 n = collision.GetContact(0).normal;
            Vector2 r = d - 2 * Vector2.Dot(d, n) * n;
            rb.velocity = Vector2.zero;
            rb.AddForce(r*lastVelocity.magnitude,ForceMode2D.Impulse);
            transform.Rotate(0f, 0f, Vector2.SignedAngle(d, r));
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
