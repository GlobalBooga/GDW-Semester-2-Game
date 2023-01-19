using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class Player : MonoBehaviour
{
    public static List<Action> onPlayerRespawn = new List<Action>();

    [Header("Attack"), Space(5f)]
    [SerializeField] private float interactRange = 1f;
    [SerializeField] private float damage = 34f;
    [SerializeField] private float attackRange = 2f;

    [Space(10f)]
    
    [Header("Movement Values"), Space(5f)]
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float accelerationDrag = 1f;
    [SerializeField] private float deccelerationDrag = 5f;
    [SerializeField] private const float gravityScale = 18f;
    private float moveSpeed;

    [Space(10f)]

    [Header("Jumping"), Space(5f)]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float airSpeedMultiplier = 0.5f;
    [SerializeField] private float jumpCooldown = 0.25f;
    public bool canJump { get; private set; }

    [Space(10f)]

    [Header("Crouching"), Space(5f)]
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float crouchYScale = 0.5f;
    private float normalYScale;

    [Space(10f)]

    [Header("Slope Movement"), Space(5f)]
    [SerializeField] private float maxSlopeAngle = 40f;

    [Space(10f)]

    [Header("Masks"), Space(5f)]
    [SerializeField] private LayerMask whatIsGround;

    [Space(10f)]
    [Header("Objects"), Space(5f)]
    private Rigidbody2D rb;
    private CapsuleCollider2D cc;
    private HPComponent hpcomp;
    public ParticleSystem particles;
    private PlayerControls controls;



    public enum MovementState
    {
        running,
        crouching,
        falling,
        idle
    }

    private MovementState currentState;


    private bool landeffect;


    internal Quaternion originalRot;
    internal Vector3 originalPos;
    internal Vector3 mousePos;


    public RaycastHit2D IsGrounded => Physics2D.CircleCast(cc.bounds.min, 0.1f, Vector2.down, 0.2f, whatIsGround);

    private float FloorAngle => Mathf.Abs(Vector2.Angle(IsGrounded.normal, Vector2.up));

    public Vector2 MoveDirection => controls.General.Move.ReadValue<Vector2>();

    public bool IsCrouching => controls.General.Crouch.IsPressed();

    public bool IsJumping => controls.General.Jump.IsPressed();


    private void OnEnable()
    {
        controls.General.Enable();
        //controls.Menus.Disable();
    }

    private void OnDisable()
    {
        controls.General.Disable();
        //controls.Menus.Enable();
    }

    private void OnDestroy()
    {
        controls.Dispose();
    }

    private void Awake()
    {
        controls = new PlayerControls();
        rb = gameObject.GetComponent<Rigidbody2D>();
        cc = gameObject.GetComponent<CapsuleCollider2D>();
        hpcomp = gameObject.GetComponent<HPComponent>();
        hpcomp.OnHPZero = OnDead;

        SetupInputEvents();
    }

    void Start()
    {
        originalPos = transform.position;
        originalRot = transform.rotation;
        normalYScale = transform.localScale.y;
        canJump = true;
        rb.freezeRotation = true;
        rb.drag = accelerationDrag;
        rb.gravityScale = gravityScale;
    }

    private void SetupInputEvents()
    {
        // JUMP
        controls.General.Jump.started += ctx => TryJump();


        // CROUCH
        controls.General.Crouch.started += ctx => Crouch();


        // UN-CROUCH
        controls.General.Crouch.canceled += ctx => UnCrouch();
    }

    private void OnDead()
    {
        Debug.Log("you died");
    }


    private void Update()
    {
        StateHandler();
        RotatePlayer();

        Debug.DrawLine(transform.position, transform.position + Vector3.Cross(IsGrounded.normal, transform.forward), Color.green, Time.deltaTime);

        if (!IsGrounded && particles && rb.velocity.y < -30f)
        {
            landeffect = true;
        }

        if (landeffect && IsGrounded)
        {
            LandEffect();
        }
    }


    #region Movement

    private void LandEffect()
    {
        landeffect = false;
        Transform obj = Instantiate(particles).transform;
        obj.position = cc.bounds.min;

        ParticleSystem pf;
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void RotatePlayer()
    {
        if (MoveDirection.x == 1)
        {
            transform.rotation = Quaternion.identity;
        }
        else if (MoveDirection.x == -1)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    private void Move()
    {
        Vector2 rawDir = MoveDirection;
        Vector2 forceDir = rawDir;

        Debug.Log(FloorAngle);
        if (FloorAngle > maxSlopeAngle)
        {
            return;
        }

        // if input is horizontal
        if (rawDir.x != 0 && IsGrounded)
        {
            forceDir = Vector3.Cross(IsGrounded.normal, transform.forward);

            rb.AddForce((Vector2.right * forceDir.x * moveSpeed * 10f), ForceMode2D.Force);
        }
        else
        {
            rb.AddForce((Vector2.right * rawDir.x * moveSpeed * 10f), ForceMode2D.Force);
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
        rb.gravityScale = gravityScale;
    }

    private void CrouchingState()
    {
        if (IsGrounded)
        {
            moveSpeed = crouchSpeed;
            rb.drag = accelerationDrag;
            rb.gravityScale = gravityScale;
        }
    }

    private void FallingState()
    {
        moveSpeed = runSpeed * airSpeedMultiplier;
        rb.drag = 0f;
        rb.gravityScale = gravityScale;
    }

    private void IdleState()
    {
        rb.drag = deccelerationDrag;

        if (FloorAngle <= maxSlopeAngle)
        {
            rb.gravityScale = 0f;
            rb.AddForce(IsGrounded.normal * -gravityScale, ForceMode2D.Force);
        }
    }

    private void StateHandler()
    {
        MovementState nextState;

        // Crouching
        if (IsCrouching)
        {
            nextState = MovementState.crouching;
        }

        // Running
        else if (IsGrounded && MoveDirection != Vector2.zero)
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

    #endregion
}