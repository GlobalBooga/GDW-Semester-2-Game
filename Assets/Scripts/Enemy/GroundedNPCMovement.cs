using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class GroundedNPCMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float accelerationDrag = 1f;
    public float deccelerationDrag = 5f;

    public float maxWaitTime = 1f;
    public float minWaitTime = 3f;

    public LayerMask pathTriggerLayer;

    private Rigidbody2D rb;
    private CapsuleCollider2D cc;

    bool pauseMovement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        rb.drag = accelerationDrag;
    }

    private void FixedUpdate()
    {
        if (!pauseMovement) Move();
    }

    void Move()
    {
        rb.AddForce(transform.right * moveSpeed * 10f,ForceMode2D.Force);

        if (Mathf.Abs(rb.velocity.x) > moveSpeed)
        {
            rb.velocity = new Vector2(transform.right.x * moveSpeed, rb.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cc.IsTouchingLayers(pathTriggerLayer))
        {
            pauseMovement = true;
            rb.drag = deccelerationDrag;
            Invoke(nameof(ResumeMovement), Random.Range(minWaitTime, maxWaitTime));
        }
    }

    void Rotate()
    {
        transform.Rotate(0f, 180f, 0f);
    }

    void ResumeMovement()
    {
        Rotate();

        pauseMovement = false;
        rb.drag = accelerationDrag;

    }
}

