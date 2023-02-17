using UnityEngine;

public class HomingMissile : Bullet
{
    private Transform target;
    private float force;
    private float maxSpeed;
    private HPComponent hp;
    public Transform body;
    public GameObject HPBars;
    public Explosive explosive;

    internal override void Awake()
    {
        if (TryGetComponent(out hp)) hp.OnHPZero = Explode;
        base.Awake();
    }

    public void Fly(Transform target, Vector2 initialDir, float initForce, float rotationForce, float maxSpeed, float damage = 0)
    {
        transform.rotation = Quaternion.identity;
        Fly(initialDir, initForce, damage);
        this.target = target;
        force = rotationForce;
        this.maxSpeed = maxSpeed;
    }

    private void Update()
    {
        Vector3 dir = target.position - body.position;
        Quaternion newQuat = Quaternion.Euler(0f, 0f, Vector3.SignedAngle(dir, Vector3.right, Vector3.back) - 90f);
        body.rotation = newQuat;
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
        Destroy(gameObject);
    }
}
