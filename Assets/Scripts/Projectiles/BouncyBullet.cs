using UnityEngine;

public class BouncyBullet : Bullet
{
    const int maxBounces = 2;
    int bounces;

    Vector2 lastVelocity;
    Quaternion lastRotation;

    internal override void Awake()
    {
        base.Awake();
        Destroy(gameObject, 15f);
    }

    private void Update()
    {
        if ((lastVelocity.magnitude == 0f && rb.linearVelocity.magnitude == 0f && lastRotation == transform.rotation) ||
            rb.linearVelocity.magnitude < speed - 2f)
        {
            Destroy(gameObject);
        }

        lastVelocity = rb.linearVelocity;
        lastRotation = transform.rotation;
    }

    internal override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == StaticHelpers.SpecialBreakableObjectLayer)
        {
            if (specialObjectDamageMultiplier > 0)
            {
                StaticHelpers.ApplyDamage(collision.gameObject, damage * specialObjectDamageMultiplier);
            }
        }
        else
        {
            if (!StaticHelpers.ApplyDamage(collision.collider.gameObject, damage))
            {
                if (StaticHelpers.ApplyDamage(collision.gameObject, damage))
                {
                    Destroy(gameObject);
                }
            }
        }

        if (bounces < maxBounces)
        {
            bounces++;
            Vector2 d = lastVelocity.normalized;
            Vector2 n = collision.GetContact(0).normal;
            Vector2 r = d - 2 * Vector2.Dot(d, n) * n;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(r*lastVelocity.magnitude,ForceMode2D.Impulse);
            transform.Rotate(0f, 0f, Vector2.SignedAngle(d, r));
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
