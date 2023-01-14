using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    private Player player;

    [Header("Movement Values"), Space(5f)]
    [SerializeField] internal float runSpeed = 10f;
    [SerializeField] internal float maxSpeed = 5f;
    [SerializeField] internal float accelerationDrag = 1f;
    [SerializeField] internal float deccelerationDrag = 5f;
    private float moveSpeed;

    [Space(10f)]

    [Header("Jumping"), Space(5f)]
    [SerializeField] internal float jumpForce = 5f;
    [SerializeField] internal float airSpeedMultiplier = 0.5f;
    [SerializeField] internal float jumpCooldown = 0.25f;
    public bool canJump { get; private set; }

    [Space(10f)]

    [Header("Crouching"), Space(5f)]
    [SerializeField] internal float crouchSpeed = 3f;
    [SerializeField] internal float crouchYScale = 0.5f;
    private float normalYScale;

    [Space(10f)]

    [Header("Slope Movement"), Space(5f)]
    [SerializeField] internal float maxSlopeAngle = 40f;
    private RaycastHit2D slopeHit;
    private bool isExitingSlope;


    [Header("Masks"), Space(5f)]
    [SerializeField] internal LayerMask whatIsGround;


    internal Rigidbody2D rb;
    internal CapsuleCollider2D cc;

    public bool IsGrounded => Physics2D.CircleCast(cc.bounds.min, 0.1f, Vector2.down, 0.2f, whatIsGround);

    public enum MovementState
    {
        running,
        crouching,
        air
    }

    private MovementState state;


    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        cc = gameObject.GetComponent<CapsuleCollider2D>();
        player = gameObject.GetComponent<Player>();
    }

    void Start()
    {
        normalYScale = transform.localScale.y;
        canJump = true;
        rb.freezeRotation = true;
        rb.drag = accelerationDrag;
    }

    private void Update()
    {
        StateHandler();
    }

    private void FixedUpdate()
    {
        // move
        Move();
    }

    private void Move()
    {
        if (!player) return;
        
        rb.AddForce(player.input.MoveDirection * moveSpeed * 10f, ForceMode2D.Force);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = player.input.MoveDirection * maxSpeed;
        }
    }

    public void Crouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        rb.AddForce(Vector2.down * 5f, ForceMode2D.Impulse);
    }

    public void UnCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, normalYScale, transform.localScale.z);
    }

    public void Jump()
    {
        canJump = false;

        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce * 10f, ForceMode2D.Impulse);

        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump()
    {
        canJump = true;
    }

    private void StateHandler()
    {
        // Crouching
        if (player.input.IsCrouching)
        {
            state = MovementState.crouching;
            if (IsGrounded)
            {
                moveSpeed = crouchSpeed;
                rb.drag = accelerationDrag;
            }
        }

        // Running
        else if (IsGrounded && player.input.MoveDirection != Vector2.zero)
        {
            state = MovementState.running;
            moveSpeed = runSpeed;
            rb.drag = accelerationDrag;
        }

        // Floating
        else if (!IsGrounded)
        {
            state = MovementState.air;
            moveSpeed = runSpeed * airSpeedMultiplier;
        }

        // Idle
        else
        {
            rb.drag = deccelerationDrag;
        }
    }
}
