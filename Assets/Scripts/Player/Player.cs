using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Attack"), Space(5f)]
    public float interactRange = 1f;
    public float damage = 34f;
    public float attackRange = 2f;

    [Space(10f)]

    [Header("Movement Values"), Space(5f)]
    public float runSpeed = 10f;
    public float moveForce = 15f;
    public float accelerationDrag = 1f;
    public float deccelerationDrag = 5f;

    [Space(10f)]

    [Header("Objects"), Space(5f)]
    private Rigidbody2D rb;
    private CircleCollider2D cc;
    private HPComponent hpcomp;
    public ParticleSystem particles;
    private PlayerControls controls;


    private Quaternion originalRot;
    private Vector3 originalPos;
    private Vector2 lastDirection;


    public Vector2 RawDirection => controls.General.Move.ReadValue<Vector2>();
    public Vector2 RotatedRawDirection => transform.up * RawDirection.y + transform.right * RawDirection.x;
    public Vector2 MousePosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    public Vector2 MouseDirection => (MousePosition - (Vector2)transform.position).normalized;
    public bool IsMoving => RawDirection != Vector2.zero;


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
        rb = GetComponent<Rigidbody2D>();
        cc = gameObject.GetComponent<CircleCollider2D>();
        hpcomp = gameObject.GetComponent<HPComponent>();

        if (hpcomp)
        { 
            hpcomp.OnHPZero = OnDead;
            hpcomp.OnHit.Add(OnHit);
        }

        SetupInputEvents();
    }

    void Start()
    {
        originalPos = transform.position;
        originalRot = transform.rotation;
        rb.freezeRotation = true;
        rb.drag = accelerationDrag;
    }

    private void Update()
    {

        transform.rotation = Quaternion.Euler(0f, 0f, Vector3.SignedAngle(MouseDirection, Vector3.up, Vector3.back));
        Debug.DrawLine(transform.position, transform.position + (Vector3)MouseDirection * 1.5f, Color.red, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Move();
    }


    #endregion

    private void Move()
    {
        bool isTooFast = Mathf.Abs(rb.velocity.magnitude) > runSpeed;
        if (!isTooFast) rb.AddForce(RawDirection * moveForce, ForceMode2D.Force);
    }

    private void SetupInputEvents()
    {
        controls.General.Attack.started += ctx =>
        {
            //GameObject.Find("Enemy").transform.Rotate(0f, 180f, 0f);
        };

        controls.General.Move.started += ctx => rb.drag = accelerationDrag;

        controls.General.Move.canceled += ctx => rb.drag = deccelerationDrag;
    }

    private void OnDead()
    {
        Debug.Log("you died");
    }

    private void OnHit()
    {
        // disable movement until grounded
        Debug.Log("ouch");
    }


    /*public static List<Action> onPlayerRespawn = new List<Action>();


    
    [SerializeField] private const float gravityScale = 10f;
    private float moveSpeed;
    private bool wasGrounded;


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



    // HELPFUL ONE LINER FUNCTIONS
    private bool IsMoving => MoveDirection.x != 0;
    public bool IsCrouching => controls.General.Crouch.IsPressed();
    public bool IsJumping => controls.General.Jump.IsPressed() || MoveDirection.y > 0;
    private RaycastHit2D GetGround => Physics2D.Raycast(cc.bounds.min, Vector2.down, 0.2f, whatIsGround);
    private bool isGrounded;


    

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


    
    */
}
