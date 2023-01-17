using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    private Player player;

    [Header("Movement Values"), Space(5f)]
    [SerializeField] internal float runSpeed = 10f;
    [SerializeField] internal float accelerationDrag = 1f;
    [SerializeField] internal float deccelerationDrag = 5f;
    private float moveSpeed;
    private float normalGravity;

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


    [Header("Masks"), Space(5f)]
    [SerializeField] internal LayerMask whatIsGround;


    internal Rigidbody2D rb;
    internal CapsuleCollider2D cc;

    public RaycastHit2D IsGrounded => Physics2D.CircleCast(cc.bounds.min, 0.2f, Vector2.down, 0.4f, whatIsGround);

    private float FloorAngle => Mathf.Abs(Vector2.Angle(IsGrounded.normal, Vector2.up));

    public enum MovementState
    {
        running,
        crouching,
        falling,
        idle
    }

    private MovementState currentState;

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
        normalGravity = rb.gravityScale;
    }

    private void Update()
    {
        StateHandler();
        RotatePlayer();
        //SlopeBehaviour();
        

        Debug.DrawLine(transform.position, transform.position + Vector3.Cross(IsGrounded.normal, transform.forward), Color.green, Time.deltaTime);

    }

    private void SlopeBehaviour()
    {
        if (IsGrounded)
        {
            rb.gravityScale = 0f;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }

    }

    private void FixedUpdate()
    {
        Move();
    }

    public void RotatePlayer()
    {
        if (player.input.MoveDirection.x == 1)
        {
            transform.rotation = Quaternion.identity;
        }
        else if (player.input.MoveDirection.x == -1)
        {
            transform.rotation = Quaternion.Euler(0f,180f, 0f);
        }
    }

    private void Move()
    {
        if (!player) return;

        Vector2 rawDir = player.input.MoveDirection;
        Vector2 forceDir = rawDir;


        if (FloorAngle > maxSlopeAngle)
        {
            return;
        }

        // if input is horizontal
        if (rawDir.x != 0 && IsGrounded)
        {
            forceDir = Vector3.Cross(IsGrounded.normal, transform.forward);

            rb.AddForce((forceDir * moveSpeed * 10f), ForceMode2D.Force);
        }
        else
        {
            rb.AddForce((rawDir * moveSpeed * 10f), ForceMode2D.Force);
        }
        
        // if input is W
        if (rawDir.y > 0)
        {
            TryJump();
        }

        // speed limiter while on ground
        if (Mathf.Abs(rb.velocity.x) > moveSpeed && IsGrounded)
        {
            rb.velocity = new Vector2(forceDir.x * moveSpeed, rb.velocity.y);
        }
        // speed limiter in air
        else 
        {
            rb.velocity = new Vector2(forceDir.x * runSpeed, rb.velocity.y);
        }
    }

    public void Crouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        if (IsGrounded) rb.AddForce(Vector2.down * 80f, ForceMode2D.Impulse);
    }

    public void UnCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, normalYScale, transform.localScale.z);
    }

    public void TryJump()
    {
        if (IsGrounded && canJump) Jump();
    }

    private void Jump()
    {
        canJump = false;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce * 10, ForceMode2D.Impulse);
        Debug.Log("jump");
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump()
    {
        canJump = true;
    }


    private void ChangeMovementState(MovementState state)
    {
        //if (state == currentState) return;

        switch (state)
        {
            case MovementState.running:
                RunningState();
                break;
            case MovementState.crouching:
                CrouchingState();
                break;
            case MovementState.falling:
                FallingState();
                break;
            case MovementState.idle:
                IdleState();
                break;
            default:
                break;
        }

        Debug.Log(state);
    }

    private void RunningState()
    {
        moveSpeed = runSpeed;
        rb.drag = accelerationDrag;
        rb.gravityScale = normalGravity;
    }

    private void CrouchingState()
    {
        if (IsGrounded)
        {
            moveSpeed = crouchSpeed;
            rb.drag = accelerationDrag;
            rb.gravityScale = normalGravity;
        }
    }

    private void FallingState()
    {
        moveSpeed = runSpeed * airSpeedMultiplier;
        rb.drag = 0f;
        rb.gravityScale = normalGravity;
    }

    private void IdleState()
    {
        rb.drag = deccelerationDrag;
        rb.gravityScale = 0f;
        rb.AddForce(IsGrounded.normal * -normalGravity, ForceMode2D.Force);
    }

    private void StateHandler()
    {
        MovementState nextState;

        // Crouching
        if (player.input.IsCrouching)
        {
            nextState = MovementState.crouching;
        }

        // Running
        else if (IsGrounded && player.input.MoveDirection != Vector2.zero)
        {
            nextState = MovementState.running;
        }

        // Falling
        else if (!IsGrounded)
        {
            nextState = MovementState.falling;
        }

        // Idle
        else
        {
            nextState = MovementState.idle;
        }

        ChangeMovementState(nextState);
    }
}
