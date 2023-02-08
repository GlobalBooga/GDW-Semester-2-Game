using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.Rendering;

public class HomingMissile : Bullet
{
    private Transform target;
    private float force;
    private float maxSpeed;
    private BoxCollider2D bc;
    private HPComponent hp;


    internal override void Awake()
    {
        base.Awake();
        bc = GetComponent<BoxCollider2D>();
        if (TryGetComponent(out hp)) hp.OnHPZero = Explode;
    }

    public void Fly(Transform target, Vector2 initialDir, float initForce, float rotationForce, float maxSpeed, float damage = 0)
    {
        Fly(initialDir, initForce, damage);
        this.target = target;
        force = rotationForce;
        this.maxSpeed = maxSpeed;
    }

    private void Update()
    {
        Vector3 dir = target.position - transform.position;
        Quaternion newQuat = Quaternion.Euler(0f, 0f, Vector3.SignedAngle(dir, Vector3.right, Vector3.back) - 90f);
        transform.rotation = newQuat;
    }

    void FixedUpdate()
    {
        bool isTooFast = Mathf.Abs(rb.velocity.magnitude) > maxSpeed;

        rb.AddForce((target.position - transform.position).normalized * force, ForceMode2D.Force);

        if (isTooFast)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    internal override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!(collision.gameObject.layer == StaticHelpers.PlayerProjectileLayer))
        {
            Explode();
        }
    }

    public void Explode()
    {
        // calculate aoe dmg
        // apply aoe damage
        Destroy(gameObject);
    }
}
