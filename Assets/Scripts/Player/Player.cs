using System;
using System.Collections.Generic;
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
    [SerializeField] private float moveForce = 15f;
    [SerializeField] private float accelerationDrag = 1f;
    [SerializeField] private float deccelerationDrag = 5f;
    [SerializeField] private const float gravityScale = 10f;
    private float moveSpeed;
    private bool wasGrounded;

    [Space(10f)]

    [Header("Jumping"), Space(5f)]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float airSpeedMultiplier = 0.5f;
    [SerializeField] private float jumpCooldown = 0.25f;
    [SerializeField] private int jumpGracePeriod = 3; // In frames!
    private bool canJump;
    private int frameCount;

    [Space(10f)]

    [Header("Crouching"), Space(5f)]
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float crouchYScale = 0.5f;
    private float normalYScale;

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
        idle,
        knockback
    }

    private MovementState currentState;

    private bool landeffect;

    private Quaternion originalRot;
    private Vector3 originalPos;
    private Vector2 lastDirection;


    // HELPFUL ONE LINER FUNCTIONS
    private bool IsMoving => MoveDirection.x != 0;
    public Vector2 MoveDirection => controls.General.Move.ReadValue<Vector2>();
    public bool IsCrouching => controls.General.Crouch.IsPressed();
    public bool IsJumping => controls.General.Jump.IsPressed() || MoveDirection.y > 0;
    private RaycastHit2D GetGround => Physics2D.Raycast(cc.bounds.min, Vector2.down, 0.2f, whatIsGround);
    private bool isGrounded;


    #region Unity Messages

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
        hpcomp.OnHit.Add(OnHit);

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

    private void Update()
    {
        isGrounded = GetGround;

        StateHandler();
        RotatePlayer();
        CheckJumping();
        JumpHelper();
        HandleLandEffect();
    }

    private void FixedUpdate()
    {
        Move();
        SpeedController();
    }

    

    #endregion

    #region Movement

    private void CheckJumping()
    {
        if (IsJumping)
        {
            TryJump();
        }
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
        if (isGrounded && !IsSignEqual(rb.velocity.x, MoveDirection.x) && IsJumping && rb.velocity.x != 0f)
        {
            Debug.Log("this");
            rb.velocity = new Vector2(0f, 0f);
        }

        // if input is horizontal
        if (isGrounded)
        {
            rb.AddForce(Vector2.right * MoveDirection.x * moveForce * 10f, ForceMode2D.Force);
        }
        else
        {
            rb.AddForce(Vector2.right * MoveDirection.x * moveForce * 10f * airSpeedMultiplier, ForceMode2D.Force);
        }
    }

    private void SpeedController()
    {
        bool isTooFast = Mathf.Abs(rb.velocity.x) > moveSpeed && IsSignEqual(MoveDirection.x, rb.velocity.x);

        
        if (isTooFast && IsMoving)
        {
            rb.velocity = new Vector2(MoveDirection.x * moveSpeed, rb.velocity.y);
        }
    }

    private bool IsSignEqual(float first, float second) => (float.IsNegative(first) && float.IsNegative(second)) || (!float.IsNegative(first) && !float.IsNegative(second));

    public void Crouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        if (isGrounded) rb.AddForce(Vector2.down * 10f, ForceMode2D.Impulse);
    }

    public void UnCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, normalYScale, transform.localScale.z);
    }

    public void TryJump()
    {
        if ((isGrounded || wasGrounded) && canJump) Jump();
    }

    private void Jump()
    {
        wasGrounded = false;
        canJump = false;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce * 10, ForceMode2D.Impulse);
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void JumpHelper()
    {
        if (isGrounded && !wasGrounded)
        {
            wasGrounded = true;
            frameCount = 0;
        }
        if (!isGrounded && wasGrounded)
        {
            if (++frameCount == jumpGracePeriod)
            {
                frameCount = 0;
                wasGrounded = false;
            }
        }
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

        //Debug.Log(state);
    }

    private void RunningState()
    {
        moveSpeed = runSpeed;
        rb.drag = accelerationDrag;
        rb.gravityScale = gravityScale;
    }

    private void CrouchingState()
    {
        if (isGrounded)
        {
            moveSpeed = crouchSpeed;
            rb.drag = accelerationDrag;
            rb.gravityScale = gravityScale;
        }
    }

    private void FallingState()
    {
        moveSpeed = runSpeed;
        rb.drag = 0f;
        rb.gravityScale = gravityScale;
    }

    private void IdleState()
    {
        rb.drag = deccelerationDrag;

        rb.velocity = new Vector2(0f, rb.velocity.y);
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
        else if (isGrounded && MoveDirection != Vector2.zero)
        {
            nextState = MovementState.running;
        }

        // Falling
        else if (!isGrounded)
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

    #region Effects
    private void LandEffect()
    {
        landeffect = false;
        Transform obj = Instantiate(particles).transform;
        obj.position = cc.bounds.min + Vector3.up * 0.1f;
    }

    private void HandleLandEffect()
    {
        if (!isGrounded && particles && rb.velocity.y < -30f)
        {
            landeffect = true;
        }

        if (landeffect && isGrounded)
        {
            LandEffect();
        }
    }

    #endregion


    private void SetupInputEvents()
    {
        // JUMP
        controls.General.Jump.started += ctx => TryJump();


        // CROUCH
        controls.General.Crouch.started += ctx => Crouch();


        // UN-CROUCH
        controls.General.Crouch.canceled += ctx => UnCrouch();

        controls.General.Attack.started += ctx =>
        {
            //GameObject.Find("Enemy").transform.Rotate(0f, 180f, 0f);
        };
    }

    private void OnDead()
    {
        Debug.Log("you died");
    }

    private void OnHit()
    {
        // disable movement until grounded
        ChangeMovementState(MovementState.knockback);
        Debug.Log("ouch");
    }
}
