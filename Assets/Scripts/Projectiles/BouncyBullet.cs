using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyBullet : Bullet
{
    const int maxBounces = 3;
    int bounces;

    internal override void Awake()
    {
        base.Awake();
    }

    internal override void OnCollisionEnter2D(Collision2D collision)
    {
        // play hit effect

        HPComponent hp;
        if (collision.transform.gameObject.TryGetComponent(out hp))
        {
            hp.Reduce(damage);
        }

        if (bounces < maxBounces)
        {
            rb.velocity = new Vector2(-rb.velocity.y, rb.velocity.x);
            bounces++;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
