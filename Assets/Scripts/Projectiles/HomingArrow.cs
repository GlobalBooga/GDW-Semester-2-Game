using UnityEngine;

public class HomingArrow : HomingMissile
{
    Animator animator;

    internal override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    public override void Fly(Transform target, Vector2 initialDir, float rotationForce, float maxSpeed, float damage = 0)
    {
        base.Fly(target, initialDir, rotationForce, maxSpeed, damage);
    }

    internal override void Update()
    {
        if (transform.parent)
        {
            transform.position = transform.parent.position;
        }
        if (target)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, Vector3.SignedAngle(rb.velocity, Vector3.up, Vector3.back)); ;
        }
        else if (!target && !transform.parent)
        {
            Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 8f, StaticHelpers.EnemyLayer);
            if (col.Length > 0) target = col[0].transform;
        }
    }

    internal override void FixedUpdate()
    {
        if (target) base.FixedUpdate();
    }

    internal override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == StaticHelpers.EnemyLayer)
        {
            collision.gameObject.GetComponent<Enemy>().Alert(transform.position);
        }
        StaticHelpers.ApplyDamage(collision.gameObject, damage);
        Destroy(gameObject);
    }
}
