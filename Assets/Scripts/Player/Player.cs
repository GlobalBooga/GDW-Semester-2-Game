using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Attack"), Space(5f)]
    public float interactRange = 1f;

    [Space(10f)]

    [Header("Movement Values"), Space(5f)]
    public float runSpeed = 10f;
    public float moveForce = 15f;
    public float accelerationDrag = 1f;
    public float deccelerationDrag = 5f;

    [Space(10f)]

    [Header("Movement Ability"), Space(5f)]
    public float dodgeCooldown = 1f;
    public float dodgeDuration = 0.3f;
    public float dodgeForce = 5f;
    private bool isUsingMoveAbility;
    private bool canUseMoveAbility = true;

    [Space(10f)]

    [Header("Objects"), Space(5f)]
    private Rigidbody2D rb;
    private CircleCollider2D cc;
    private HPComponent hpcomp;
    //public ParticleSystem particles;
    private PlayerControls controls;
    public Weapon weapon;
    private GameObject pickupable;
    public Animator screenOverlayAnimator;

    // for animations
    public const string PLAYER_HIT_INDICATOR = "PlayerDamageTaken";


    // other
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

        if (weapon) weapon.SetHeld(); 
    }

    private void Update()
    {
        // If we are still holding down attack button, continue attacking
        if (weapon)
        {
            if (weapon.readyToUse && weapon.isAutoUse && controls.General.Attack.IsPressed()) 
                weapon.Use();
        }

        transform.rotation = Quaternion.Euler(0f, 0f, Vector3.SignedAngle(MouseDirection, Vector3.up, Vector3.back));
        Debug.DrawLine(transform.position, transform.position + (Vector3)MouseDirection * 1.5f, Color.red, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!isUsingMoveAbility) Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.layer);
        //Debug.Log(LayerMask.LayerToName(StaticHelpers.PickupLayer));
        if (collision.gameObject.layer == StaticHelpers.PickupLayer)
        {
            pickupable = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        pickupable = null;
    }


    #endregion

    private void Move()
    {
        //bool movingInSameDir;
        bool isTooFast = Mathf.Abs(rb.velocity.magnitude) > runSpeed;
        rb.AddForce(RawDirection * moveForce * rb.mass, ForceMode2D.Force); //if (!isTooFast) 

        if (isTooFast)
        {
            rb.velocity = rb.velocity.normalized * runSpeed;
        }

        if (RawDirection == Vector2.zero) rb.drag = deccelerationDrag;
    }

    private void SetupInputEvents()
    {
        controls.General.Move.started += ctx => rb.drag = accelerationDrag;

        controls.General.Move.canceled += ctx => rb.drag = deccelerationDrag;

        controls.General.Pickup.started += ctx => 
        {
            if (pickupable)
            {
                Weapon newWeapon = (Weapon)pickupable.GetComponent(typeof(Weapon));

                if (newWeapon)
                {
                    weapon.Drop(transform.up);
                
                    newWeapon.Pickup(transform, weapon);
                    weapon = newWeapon;
                }
            }
        };

        controls.General.Attack.started += ctx => { if (weapon) weapon.Use(); };

        controls.General.Reload.started += ctx => { };

        controls.General.MovementAbility.started += ctx =>
        {
            if (!canUseMoveAbility) return;
            canUseMoveAbility = false;
            isUsingMoveAbility = true;

            if (hpcomp) hpcomp.isInvincible = true;
            gameObject.layer = StaticHelpers.PlayerInvincibleLayer;

            Vector2 dir = rb.velocity.normalized;
            rb.velocity = Vector2.zero;
            if (RawDirection == Vector2.zero)
            {
                // dash backwards
                rb.AddForce((dir - (Vector2)transform.up * 2.5f).normalized * dodgeForce * rb.mass, ForceMode2D.Impulse);
            }
            else
            {
                // dash in direction
                rb.AddForce((dir + RawDirection * 2.5f).normalized * dodgeForce * rb.mass, ForceMode2D.Impulse);
            }
            rb.drag = 10f;
            Invoke(nameof(EndDodge), dodgeDuration);
            
        };
        
        controls.General.Ultimate.started += ctx => { };
        
        controls.General.WeaponAbility.started += ctx => { };
    }

    private void EndDodge()
    {
        rb.drag = accelerationDrag;
        isUsingMoveAbility = false;
        if (hpcomp) hpcomp.isInvincible = false;
        gameObject.layer = StaticHelpers.PlayerLayer;

        if (dodgeCooldown - dodgeDuration > 0) Invoke(nameof(ResetMoveAbility), dodgeCooldown - dodgeDuration);
        else ResetMoveAbility();
    }

    private void ResetMoveAbility()
    {
        canUseMoveAbility = true;
    }

    private void OnDead()
    {
        Debug.Log("you died");
    }

    private void OnHit()
    {
        // disable movement until grounded
        //Debug.Log("ouch");
        if (screenOverlayAnimator) screenOverlayAnimator.Play(PLAYER_HIT_INDICATOR);
    }
}
