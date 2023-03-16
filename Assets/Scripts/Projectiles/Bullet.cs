using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Bullet : MonoBehaviour
{
    internal Rigidbody2D rb;
    internal float damage;
    public float specialObjectDamageMultiplier;

    internal virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 10f);
    }

    public void Fly(Vector2 dir, float force, float damage = 0)
    {
        if (!rb) return;

        rb.AddForce(dir * force, ForceMode2D.Impulse);
        this.damage = damage;
        transform.parent = null;
    }


    internal virtual void OnCollisionEnter2D(Collision2D collision)
    {

        // play hit effect
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
                StaticHelpers.ApplyDamage(collision.gameObject, damage);
            }
        }

        Destroy(gameObject);
    }
}
