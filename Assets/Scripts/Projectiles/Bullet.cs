using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Bullet : MonoBehaviour
{
    internal Rigidbody2D rb;
    internal float damage;

    internal virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Fly(Vector2 dir, float force, float damage = 0)
    {
        rb.AddForce(dir * force, ForceMode2D.Impulse);
        this.damage = damage;
        transform.parent = null;
    }


    internal virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // play hit effect

        StaticHelpers.ApplyDamage(collision.gameObject, damage);

        Destroy(gameObject);
    }
}
