using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

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
    public Vector2 knockBackForce;
    //public float attackArcAngle;
    private bool attackReady;
    private bool hit;
    private bool isAttacking;
    [Space(10f)]

    [Header("Detection"), Space(5f)]
    public float reactionTime;
    public float viewDistance;
    public float minSearchTime;
    public float maxSearchTime;
    private bool discoveredPlayer;
    private bool isChasingPlayer;
    [Space(10f)]

    [Header("Search"), Space(5f)]
    public float maxWaitTime = 1f;
    public float minWaitTime = 3f;
    [Space(10f)]

    [Header("Movement"), Space(5f)]
    public float walkSpeed = 3f;
    public float chaseSpeed = 8f;
    private float moveSpeed;
    public float moveForce = 15f;
    public float accelerationDrag = 1f;
    public float deccelerationDrag = 5f;
    private bool isOnEdge;
    [Space(10f)]

    [Header("Masks"), Space(5f)]
    public LayerMask pathTriggerLayer;
    private const int pathTriggerLayerValue = 7;
    public LayerMask whatIsPlayer;
    public LayerMask whatIsGround;
    public LayerMask whatBlocksSight;
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

    #region Unity Messages

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
        moveSpeed = walkSpeed;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        PlayerDetectionHandler();
        AttackHandler();
    }
    
    private void FixedUpdate()
    {
        MovementHandler();   
    }

    private void OnValidate()
    {
        if (attackCooldown < damageApplyDelay + damageableTimeWindow)
        {
            attackCooldown = damageApplyDelay + damageableTimeWindow + 0.1f;
        }
    }

    #endregion

    #region Movement

    private void MovementHandler()
    {
        if ((isChasingPlayer && Vector3.Distance(transform.position, playerLoc.position) < 2f) ||
            (isChasingPlayer && !IsOnSameGroundAsPlayer() && isOnEdge))
        {
            PauseMovement();
        }
        else if (pauseMovement && discoveredPlayer && !isChasingPlayer)
        {
            ResumeMovement();
        }
        /*else if (pauseMovement)
        {
            RotateAndResumeWalking();
        }*/

        if (discoveredPlayer && !IsFacingPlayer) Rotate();
       
        if (!pauseMovement) Move();
    }

    private void Move()
    {
        rb.AddForce(transform.right * moveForce * 10f, ForceMode2D.Force);

        // SPEED CONTROL
        if (Mathf.Abs(rb.velocity.x) > moveSpeed)
        {
            rb.velocity = new Vector2(transform.right.x * moveSpeed, rb.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cc.IsTouchingLayers(pathTriggerLayer))
        {
            isOnEdge = true;

            if (!isChasingPlayer)
            {
                PauseMovement();
                Invoke(nameof(RotateAndResumeWalking), Random.Range(minWaitTime, maxWaitTime));
            }
        }

        if (attackArea.IsTouchingLayers(whatIsPlayer) && !hit)
        {
            hit = true;
            ApplyDamage(collision.gameObject);
        }
    }

    /*private void OnTriggerStay2D(Collider2D collision)
    {
        if (cc.IsTouchingLayers(pathTriggerLayer))
        {
            ReactToEdge();
        }
    }*/

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == pathTriggerLayerValue)
        {
            isOnEdge = false;
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

    private bool IsPlayerVisible()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, playerLoc.position - transform.position, viewDistance, whatBlocksSight);
        if (hit)
        {
            if (hit.transform.gameObject == playerLoc.gameObject)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsOnSameGroundAsPlayer()
    {
        RaycastHit2D ground = Physics2D.Raycast(transform.position, Vector2.down, 1.3f, whatIsGround);
        RaycastHit2D playerGround = Physics2D.Raycast(playerLoc.position, Vector2.down, 4f, whatIsGround); 

        if (ground && playerGround)
        {
            if (ground.transform.gameObject == playerGround.transform.gameObject)
            {
                //Debug.Log("is on same ground");
                return true;
            }
        }
        return false;
    }

    private void PlayerDetectionHandler()
    {
        //Debug.Log("playerdetectionhandler");
        if (CanSeePlayer() && IsOnSameGroundAsPlayer() && !isChasingPlayer)
        {
            ChasePlayer();
            //Debug.Log("chase");
        }
        else /*if (!CanSeePlayer() && isChasingPlayer)*/
        {
            if (!isOnEdge) Invoke(nameof(EndChase), Random.Range(minSearchTime, maxSearchTime));
            else
            {
                EndChase();
                Rotate();
            }
        }
    }

    private bool CanSeePlayer()
    {
        if (IsPlayerInRange && IsFacingPlayer)
        {
            // avoid raycasts when possible
            if (IsPlayerVisible())
            {
                return true;
            }
        }

        return false;
    }

    private void ChasePlayer()
    {
        isChasingPlayer = true;
        discoveredPlayer = true;
        moveSpeed = chaseSpeed;
    }

    private void EndChase()
    {
        if (CanSeePlayer() && IsOnSameGroundAsPlayer()) return;

        discoveredPlayer = false;
        isChasingPlayer = false;
        moveSpeed = walkSpeed;
    }

    /*private void ReactToEdge()
    {
        Vector2 dir = new(transform.right.x * 1f, -1f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir.normalized, 1.5f, whatIsGround);
        if (hit) isOnSameGroundAsPlayer = true;
        else isOnSameGroundAsPlayer = false;

        Debug.DrawLine(transform.position, (Vector2)transform.position + dir.normalized * 1.5f, Color.red, 1f);
    }*/

    #endregion

    #region Aggressive Behaviour

    private bool CanAttack => CanSeePlayer() && Vector3.Distance(playerLoc.position, transform.position) <= attackRange;
    private void ResetAttack() => attackReady = true;

    private void AttackHandler()
    {
        if (CanAttack && attackReady && !isAttacking)
        {
            isAttacking = true;
            PauseMovement();
            Invoke(nameof(Attack), attackDelay);
        }
    }


    private void Attack()
    {
        hit = false;
        attackReady = false;

        if (animator) animator.Play(ANIM_ATTACK);

        Invoke(nameof(EnableDamageZone), damageApplyDelay);
        Invoke(nameof(ResetAttack), attackCooldown);
        if (!CanAttack) Invoke(nameof(ResumeMovement), damageApplyDelay + damageableTimeWindow + 0.2f);
    }

    private void EnableDamageZone()
    {
        attackArea.enabled = true;
        Invoke(nameof(EndAttack), damageableTimeWindow);
    }

    private void EndAttack()
    {
        attackArea.enabled = false;
        isAttacking = false;
    }

    private void ApplyDamage(GameObject other)
    {
        HPComponent hp;
        if (other.TryGetComponent(out hp))
        {
            hp.Reduce(attackDamage);
            if (!hp.isInvincible) ApplyKnockback(other.GetComponent<Rigidbody2D>());
        }
    }

    private void ApplyKnockback(Rigidbody2D body)
    {
        if (body) body.AddForce(knockBackForce, ForceMode2D.Impulse);
    }
    

    #endregion

    #region Animations

    private const string ANIM_ATTACK = "EnemyAttack";


    #endregion
}