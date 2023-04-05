using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(HPComponent))]
public class Enemy : MonoBehaviour
{
    internal bool lockedOnPlayer;
    internal float lookSpeed = 0.01f;
    internal float rotationTime;
    internal float fov;
    internal bool foundPlayer;
    internal Queue<Vector3> playerPoses = new();
    private float timeSinceLastPos = 0;
    internal float totalTravelDist;
    internal float lerpStartTime;
    internal Vector3 lerpStart;
    internal Vector3 nextPos;
    private int frames;
    private int detectionRate = 50; // the average
    private bool isInspecting;
    private bool isAlerted;
    private Vector3 sightMax;
    private Vector3 sightMin;

    internal float retreatSpeed;
    internal float attackMovementSpeed;
    internal float runSpeed;
    internal float walkSpeed;
    private float moveSpeed;

    internal float minAttackDistance;
    internal float maxAttackDistance;
    internal float comfortableAttackDist;
    internal float attackDistance;
    internal bool canAttack = false; // are we in the right position to attack
    internal bool attackReady = true; // cooldowns?
    internal bool isAttacking;
    internal bool lockRotation;
    private bool isDead;

    [Space(10f)]
    [Header("Scriptable Object"), Space(5f)]
    public EnemyScriptableObject eso;


    [Space(10f)]
    [Header("Objects"), Space(5f)]
    public Transform body;

    private SpriteRenderer sr;
    internal Rigidbody2D rb;
    internal Transform playerLoc;
    internal HPComponent hp;
    private Vector3 originalPos;
    private Quaternion originalRot;
    public Animator animator;

    [Space(10f)]
    [Header("Tutorial"), Space(5f)]
    public bool isDummy;
    public float reviveDelay = 2f;

    private bool reviving;

    public Vector3 PlayerDirection => playerLoc.position - transform.position;
    public float PlayerDistance => Vector3.Distance(transform.position, playerLoc.position);


    internal virtual void OnDisable()
    {
        transform.position = originalPos;
        body.rotation = originalRot;
        foundPlayer = false;
        lockedOnPlayer = false;
        canAttack = false;
        ResetAttack();
        SetDefaultBehaviour();
        playerPoses.Clear();
    }

    internal virtual void Awake()
    {
        originalPos = transform.position;
        originalRot = transform.rotation;
        rb = GetComponent<Rigidbody2D>();
        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        playerLoc = GameObject.Find("Player").transform;
        //cc = GetComponent<CircleCollider2D>();
        // set the ondied func
        if (TryGetComponent(out hp)) 
        {
            hp.OnHPZero = OnDied;
            hp.OnHit.Add(Hit);
        }
    }

    internal virtual void Start()
    {
        originalPos = transform.position;
        originalRot = body.rotation;

        SetDefaultBehaviour();

        NewDetectionRate();
        NewAttackDistance();
    }

    internal void SetDefaultBehaviour()
    {
        rb.drag = eso.defaultDeccelerationDrag;
        fov = eso.normalFOV;
        comfortableAttackDist = eso.defaultComfortableAttackDist;
        minAttackDistance = eso.defaultMinAttackDistance;
        maxAttackDistance = eso.defaultMaxAttackDistance;
        runSpeed = eso.defaultRunSpeed;
        walkSpeed = eso.defualtWalkSpeed;
        retreatSpeed = eso.defaultRetreatSpeed;
        attackMovementSpeed = eso.defaultAttackMovementSpeed;
        eso.chasePlayer = true;
        lockRotation = false;
    }

    internal virtual void Update()
    {
        if (isDummy) return;

        UpdateDetection();

        // base attack logic
        if (canAttack && attackReady)
        {
            attackReady = false;
            Attack();
        }
    }

    internal void UpdateDetection()
    {
        HandleSight();

        if (eso.chasePlayer)
        {
            RecordPlayerPosition();

            // If we are chasing the player, look at the last known point
            if (!eso.canSeeThroughWalls && !foundPlayer && !isAlerted && playerPoses.Count > 0f) LookAt(playerPoses.Last());
        }
    }

    internal virtual void FixedUpdate()
    {
        if (isDummy) return;

        // Standard chase player
        if (eso.chasePlayer && lockedOnPlayer && PlayerDistance > attackDistance)
        {
            ChasePlayer();
        }
        // if player is getting too close
        else if (lockedOnPlayer && PlayerDistance < comfortableAttackDist)
        {
            Retreat();   
        }
        // if we are in the comfortable attack zone
        // or if we just havent discovered the player yet
        else
        {
            Stay();
        }

        // if we lost the player and is following their tacks
        if (eso.chasePlayer && !foundPlayer && playerPoses.Count > 0f)
        {
            SearchForPlayer();
        }
    }

    private void QueuePlayerPos()
    {
        // If the player is too close from the last logged position
        if (playerPoses.Count > 0)
        {
            if (Vector3.Distance(playerLoc.position, playerPoses.Last()) < eso.minDistanceBetweenPoses) return;
        }

        //saveOneMorePos = false;

        if (playerPoses.Count > eso.maxStoredPoses) playerPoses.Dequeue();
        playerPoses.Enqueue(playerLoc.position);
    }

    internal virtual void RecordPlayerPosition()
    {
        // Player position recorder
        if (timeSinceLastPos >= eso.secondsBetweenPoses)
        {
            timeSinceLastPos = 0f;
            QueuePlayerPos();
        }
    }

    internal void ChasePlayer()
    {
        moveSpeed = isAttacking ? attackMovementSpeed : runSpeed;
        bool isTooFast = Mathf.Abs(rb.velocity.magnitude) > moveSpeed;
        if (!isTooFast) Move();

        // dont attack if chasing - calls once at a time
        if (canAttack)
        {
            canAttack = false;
            NewAttackDistance();
        }
    }

    internal void Retreat()
    {
        moveSpeed = -retreatSpeed;
        bool isTooFast = Mathf.Abs(rb.velocity.magnitude) > Mathf.Abs(moveSpeed);
        if (!isTooFast) Move();
    }

    internal void Stay()
    {
        rb.drag = eso.defaultDeccelerationDrag;

        if (lockedOnPlayer && !canAttack)
        {
            canAttack = true;
        }
    }

    internal void SearchForPlayer()
    {
        float distCovered = (Time.time - lerpStartTime) * runSpeed;
        if (totalTravelDist > 0) transform.position = Vector3.Lerp(lerpStart, nextPos, eso.test.Evaluate(distCovered / totalTravelDist));

        if (Vector3.Distance(transform.position, nextPos) < 0.05f)
        {
            eso.test = AnimationCurve.Linear(0, 0, 1, 1);
            // dispose of the last pos
            playerPoses.Dequeue();

            // check for the next one
            if (playerPoses.TryPeek(out nextPos))
            {
                totalTravelDist = Vector3.Distance(transform.position, nextPos);
                lerpStartTime = Time.time;
                lerpStart = transform.position;
                rotationTime = 0f;
            }

            // when we arrive at the last pos
            else if (eso.viewDistance > 0)
            {
                StartCoroutine(nameof(InspectSurroundings));
            }
        }
    }

    internal void HandleSight()
    {
        if (eso.viewDistance == 0) return;

        // ***********************  vv  *****************
        // Perform detection in INTERVALS for performance
        if (frames >= detectionRate)
        {
            frames = 0;

            // if the player is too close and we havent see them yet - called once
            if (!lockedOnPlayer && CanSeePlayer(360f, eso.instantDetectDist))
            {
                lockedOnPlayer = true;
                rotationTime = 0f;
            }

            // basic can see player check
            if (CanSeePlayer())
            {
                // if its the first time we see the player - called once
                if (!foundPlayer)
                {
                    OnPlayerFound();
                }
            }
            // if we can't see the player anymore - called once
            else if (foundPlayer)
            {
                OnLostSightOfPlayer();

                if (playerPoses.Count > 0 && eso.chasePlayer)
                {
                    StartPlayerSearch();
                }
            }
        }

        // *******************************************  vv  *******
        // if we are already looking at player - called EVERY FRAME
        if (lockedOnPlayer)
        {
            //if (LookAt(playerLoc.position)) rotationTime = 0f;
            LookAt(playerLoc.position);

            timeSinceLastPos += Time.deltaTime;
        }
        frames++;
    }

    /// <summary>
    /// lerped rotation for smoothness. Must be called from Update(). 
    /// </summary>
    /// <param name="point">the point to rotate towards</param>
    /// <returns>True when rotation is completed</returns>
    internal virtual bool LookAt(Vector2 point) 
    {
        if (lockRotation) return true;

        rotationTime++;
        Vector2 thing = point - (Vector2)transform.position;
        Quaternion newQuat = Quaternion.Euler(0f, 0f, Vector3.SignedAngle(thing, Vector3.up, Vector3.back));
        body.rotation = Quaternion.Lerp(body.rotation, newQuat, eso.rotationCurve.Evaluate(rotationTime * lookSpeed));
        return rotationTime * lookSpeed > 1f;
    }

    internal virtual IEnumerator InspectSurroundings()
    {
        isInspecting = true;

        // speed up detection rate even more
        NewDetectionRate(1 / (eso.searchDetectionRateMult * 1.5f));

        Vector3 right = transform.position - transform.up;
        Vector3 left = transform.position + transform.up;
        Vector3 forward = transform.position + body.up;

        rotationTime = 0;
        while (!LookAt(right)) yield return null;
        
        rotationTime = 0;
        while (!LookAt(left)) yield return null;
        
        rotationTime = 0;
        while (!LookAt(forward)) yield return null;

        // when finished chasing, reset detection
        NewDetectionRate();
        fov = eso.normalFOV;
        isInspecting = false;
    }

    internal virtual void OnPlayerFound()
    {
        // Immediately store the player's position
        if (eso.chasePlayer)
        {
            playerPoses.Clear();
            QueuePlayerPos();
        }

        foundPlayer = true;
        rotationTime = 0f;

        Invoke(nameof(OnPlayerFoundDelayed), eso.defaultReactionTime);

        if (!LevelManager.instance.playerFound)
        {
            LevelManager.instance.playerFound = true;
        }
    }

    /// <summary>
    /// OnPlayerFound after reaction time
    /// </summary>
    internal virtual void OnPlayerFoundDelayed()
    {
        if (!foundPlayer) return;

        // lock on to player
        lockedOnPlayer = true;
        moveSpeed = eso.defaultRunSpeed;
        NewAttackDistance();
    }

    internal virtual void OnLostSightOfPlayer()
    {
        // cancel any ongoing inspeciton
        if (isInspecting) StopAllCoroutines();

        // increase the detection rate
        if (eso.chasePlayer) NewDetectionRate(1 / eso.searchDetectionRateMult);

        eso.test = AnimationCurve.EaseInOut(0, 0, 1, 1);

        // Change some detection related properties
        foundPlayer = false;
        if (!eso.canSeeThroughWalls) lockedOnPlayer = false;
        rotationTime = 0f;
        moveSpeed = walkSpeed;
        if (eso.chasePlayer) fov = eso.searchFOV;

        // prevent attacking
        canAttack = false;
    }

    /// <summary>
    /// Start following the stored player positions
    /// </summary>
    internal virtual void StartPlayerSearch()
    {
        // finding the quickest route
        Queue<Vector3> tempQ = new();
        Stack<Vector3> tempStk = new();
        Vector3 shortcut = Vector3.zero;

        // Scan our recorded player poses from first to last
        foreach (Vector3 pos in playerPoses.Reverse())
        {
            bool clear = false;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (pos - transform.position).normalized, eso.viewDistance, eso.whatIsWall);
            if (hit)
            {
                float hitDist = Vector2.Distance(hit.point, (Vector2)transform.position);
                float posDist = Vector3.Distance(pos, transform.position);

                // check if we have a clear path to the position we are checking
                if (hitDist > posDist) clear = true;
                else clear = false;
            }

            if (clear)
            {
                shortcut = pos;
                break;
            }
            else
            {
                // if we can't get to it
                tempStk.Push(pos);
            }
        }

        // check if we got stuff
        if (shortcut != Vector3.zero)
        {
            tempQ.Enqueue(shortcut);

            int len = tempStk.Count;
            for (int i = 0; i < len; i++)
            {
                tempQ.Enqueue(tempStk.Pop());
            }
            if (tempQ.Count > 0) playerPoses = tempQ;
        }

        //saveOneMorePos = true;

        nextPos = playerPoses.Peek();

        // set the next position to go to
        totalTravelDist = Vector3.Distance(transform.position, nextPos);
        lerpStartTime = Time.time;
        lerpStart = transform.position;
        rotationTime = 0f;
    }

    internal virtual void NewDetectionRate(float multiplier = 1f)
    {
        detectionRate = Mathf.RoundToInt(Random.Range(detectionRate + 10, detectionRate - 10) * multiplier);
    }

    /// <summary>
    /// Set the attack distance to a random new variable between the max and min attack distance
    /// </summary>
    internal virtual void NewAttackDistance()
    {
        attackDistance = Random.Range(minAttackDistance, maxAttackDistance);
    }

    public bool CanSeePlayer()
    {
        return CanSeePlayer(fov, eso.viewDistance);
    }

    public bool CanSeePlayer(float scanAngle, float scanDist)
    {
        // is in range?
        if (PlayerDistance <= scanDist)
        {
            // is in fov?
            if (Vector3.Angle(body.up, playerLoc.position - transform.position) <= scanAngle * 0.5f)
            {
                // is view blocked?
                RaycastHit2D hit = Physics2D.Raycast(transform.position, (playerLoc.position - transform.position).normalized, scanDist, eso.whatBlocksSight);
                if (hit)
                {
                    if (hit.transform.gameObject.name == "Player")
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    internal virtual void Move()
    {
        rb.drag = eso.defaultAccelerationDrag;
        if (moveSpeed > 0)
        {
            Vector2 currentdir = rb.velocity;
            Vector2 desireddir = body.up;
            Vector3 correction = desireddir - currentdir;
            rb.AddForce((body.up * eso.defaultMoveForce * rb.mass) + (correction * eso.snappyness), ForceMode2D.Force);
        }
        else rb.AddForce(body.up * -eso.defaultMoveForce * rb.mass, ForceMode2D.Force);
    }

    internal virtual void Attack()
    {
        isAttacking = true;
    }

    internal virtual void ResetAttack()
    {
        attackReady = true;
        isAttacking = false;
    }

    public virtual void OnDied()
    {
        if (isDummy) 
        {
            // dont die
            rb.simulated = false;
            sr.color = new Color(1,1,1,0.3f);
            hp.isInvincible = true;
            if (!reviving)
            {
                reviving = true;
                Invoke(nameof(Revive), reviveDelay);
            }
            return;
        }

        if (!isDead)
        {
            isDead = true;
            LevelManager.instance.EnemyDied();
            Destroy(gameObject);
        }
    }

    // for tutorial
    public void Revive()
    {
        rb.simulated = true;
        hp.isInvincible = false;
        sr.color = Color.white;
        hp.Add(hp.maxHealth);
        reviving = false;
    }

    public void Alert(Vector3 lookAt)
    {
        if (lockedOnPlayer || !isActiveAndEnabled) return;

        if (isInspecting) StopAllCoroutines();
        StartCoroutine(nameof(AlertTurnTo), lookAt);
    }

    private IEnumerator AlertTurnTo(Vector3 loc)
    {
        isAlerted = true;

        rotationTime = 0f;
        while (!LookAt(loc))
        {
            yield return null;
        }

        isAlerted = false;
    }    

    internal virtual void Hit()
    {
        
    }
}
