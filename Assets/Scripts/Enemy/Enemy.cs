using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Aggressive Behaviour"), Space(5f)]
    public float attackDamage;
    public float attackCooldown;
    public float attackDelay;
    public float damageApplyDelay;
    public float damageableTimeWindow;
    public float attackRange;
    //public float attackArcAngle;
    private bool attackReady;
    private bool hit;
    [Space(10f)]

    [Header("Detection"), Space(5f)]
    public float reactionTime;
    public float viewDistance;
    //public float viewArc;
    public float maxSearchTime;
    [Space(10f)]

    [Header("Search"), Space(5f)]
    public float maxWaitTime = 1f;
    public float minWaitTime = 3f;
    [Space(10f)]

    [Header("Movement"), Space(5f)]
    public float moveSpeed = 1f;
    public float walkSpeed = 1f;
    public float chaseSpeed;
    public float accelerationDrag = 1f;
    public float deccelerationDrag = 5f;
    [Space(10f)]

    [Header("Masks"), Space(5f)]
    public LayerMask pathTriggerLayer;
    public LayerMask whatIsPlayer;
    [Space(10f)]

    [Header("Preset"), Space(5f)]
    public EnemyScriptableObject enemyScriptable;

    // OBJECTS
    [Header("Objects"), Space(5f)]
    public Animator animator;
    public SpriteRenderer sr;
    public PolygonCollider2D attackArea;
    private Rigidbody2D rb;
    private CapsuleCollider2D cc;
    private Transform playerLoc;

    private bool pauseMovement;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        playerLoc = GameObject.Find("Player").transform;
    }

    void Start()
    {
        rb.drag = accelerationDrag;
        rb.freezeRotation = true;
        attackReady = true;
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            if (CanChasePlayer())
            ChasePlayer();
        }

        if (CanAttack)
        {
            PauseMovement();
            Invoke(nameof(Attack), attackDelay);
        }

    }

    private void OnValidate()
    {
        if (attackCooldown < damageApplyDelay + damageableTimeWindow)
        {
            attackCooldown = damageApplyDelay + damageableTimeWindow + 0.1f;
        }
    }


    #region Movement

    private void FixedUpdate()
    {
        if (!pauseMovement) Move();
    }

    void Move()
    {

        rb.AddForce(transform.right * moveSpeed * 10f, ForceMode2D.Force);

        if (Mathf.Abs(rb.velocity.x) > moveSpeed)
        {
            rb.velocity = new Vector2(transform.right.x * moveSpeed, rb.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cc.IsTouchingLayers(pathTriggerLayer))
        {
            PauseMovement();
            Invoke(nameof(RotateAndResumeWalking), Random.Range(minWaitTime, maxWaitTime));
        }

        if (attackArea.IsTouchingLayers(whatIsPlayer) && !hit)
        {
            hit = true;
            ApplyDamage(collision.gameObject);
        }
    }


    private void PauseMovement()
    {
        pauseMovement = true;
        rb.drag = deccelerationDrag;
    }

    private void RotateAndResumeWalking()
    {
        Rotate();
        ResumeMovement();
    }

    void ResumeMovement()
    {
        pauseMovement = false;
        rb.drag = accelerationDrag;
    }

    void Rotate() => transform.Rotate(0f, 180f, 0f);


    #endregion

    #region Detection

    private bool IsPlayerInRange => Vector3.Distance(transform.position, playerLoc.position) < viewDistance;
    private bool IsFacingPlayer => ((transform.position - playerLoc.position) * transform.right.x).x < 0;
    private bool IsPlayerVisible => Physics2D.Raycast(transform.position, playerLoc.position - transform.position, viewDistance, whatIsPlayer);

    private bool CanChasePlayer()
    {
        return false;
    }


    private bool CanSeePlayer()
    {
        if (IsPlayerInRange && IsFacingPlayer)
        {
            // avoid raycasts when possible
            if (IsPlayerVisible)
            {
                return true;
            }
        }

        return false;
    }

    private void ChasePlayer()
    {

    }


    #endregion

    #region Aggressive Behaviour

    private bool CanAttack => CanSeePlayer() && Vector3.Distance(playerLoc.position, transform.position) <= attackRange && attackReady;
    private void ResetAttack() => attackReady = true;

    private void Attack()
    {
        hit = false;
        attackReady = false;

        if (animator) animator.Play(ANIM_ATTACK);

        Invoke(nameof(EnableDamageZone), damageApplyDelay);
        Invoke(nameof(ResetAttack), attackCooldown);
        Invoke(nameof(ResumeMovement), damageApplyDelay + damageableTimeWindow + 0.2f);
    }

    private void EnableDamageZone()
    {
        attackArea.enabled = true;
        Invoke(nameof(EndAttack), damageableTimeWindow);
    }

    private void EndAttack()
    {
        attackArea.enabled = false;
    }

    private void ApplyDamage(GameObject other)
    {
        HPComponent hp;
        if (other.TryGetComponent<HPComponent>(out hp))
        {
            hp.Reduce(attackDamage);
        }

        other.GetComponent<Rigidbody2D>().AddForce((transform.right * 300f) + (Vector3.up * 50f), ForceMode2D.Impulse);
    }

    #endregion

    #region Animations

    private const string ANIM_ATTACK = "EnemyAttack";


    #endregion
}