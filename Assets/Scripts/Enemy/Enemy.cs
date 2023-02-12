using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(HPComponent))]
public class Enemy : MonoBehaviour
{
    [Header("Debug"), Space(5f)]
    public bool showDebugStuff;
    public bool showForwards = true;

    [Space(10f)]

    [Header("Detection"), Space(5f)]
    public float reactionTime = 0f;
    public float viewDistance = 40f;
    public float instantDetectDist = 3f;
    public float normalFOV = 60f;
    public float searchFOV = 120f;
    private float fov;
    private bool foundPlayer;
    internal bool lockedOnPlayer;
    internal float lookSpeed = 0.01f;
    public AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Queue<Vector3> playerPoses = new();
    public int maxStoredPoses = 4;
    public float secondsBetweenPoses = 1f;
    private float timeSinceLastPos = 0;

    private float totalTravelDist;
    private float lerpStartTime;
    private Vector3 lerpStart;
    private Vector3 nextPos;
    public float minDistanceBetweenPoses = 1f;
    private bool saveOneMorePos;

    private int frames;
    private int detectionRate = 50; // the average
    public float searchDetectionRateMult = 1f;
    Vector3 sightMax;
    Vector3 sightMin;
    internal float rotationTime;
    public bool canSeeThroughWalls;
    private bool isInspecting;
    private bool isAlerted;

    [Space(10f)]

    [Header("Movement"), Space(5f)]
    public float walkSpeed = 3f;
    public float runSpeed = 8f;
    public float attackMovementSpeed = 3f;
    public float retreatSpeed = 3f;
    private float moveSpeed;
    public float moveForce = 15f;
    public float accelerationDrag = 1f;
    public float deccelerationDrag = 5f;
    [Space(10f)]

    [Header("Aggressive Behaviour"), Space(5f)]
    public float maxAttackDistance = 10f;
    public float minAttackDistance = 6f;
    private float attackDistance;
    public float comfortableAttackDist;
    internal bool canAttack = false; // are we in the right position to attack
    internal bool attackReady = true; // cooldowns?
    internal bool isAttacking;
    internal bool lockRotation;
    public bool chasePlayer = true;


    [Space(10f)]

    [Header("Masks"), Space(5f)]
    public LayerMask whatIsPlayer;
    public LayerMask whatIsWall;
    public LayerMask whatBlocksSight;
    public LayerMask whatTakesDamage;
    [Space(10f)]


    [Header("Objects"), Space(5f)]
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    //private CircleCollider2D cc;
    internal Transform playerLoc;
    public Transform body;
    internal HPComponent hp;
    private Vector3 originalPos;
    private Quaternion originalRot;
    public Vector3 PlayerDirection => playerLoc.position - transform.position;
    public float PlayerDistance => Vector3.Distance(transform.position, playerLoc.position);





    EnemyScriptableObject eso;






    private void QueuePlayerPos()
    {
        // If the player is too close from the last logged position
        if (playerPoses.Count > 0)
        {
            if (Vector3.Distance(playerLoc.position, playerPoses.Last()) < minDistanceBetweenPoses) return;
        }

        saveOneMorePos = false;

        if (playerPoses.Count > maxStoredPoses) playerPoses.Dequeue();
        if (showDebugStuff) DrawDebugCross(playerLoc.position, secondsBetweenPoses * maxStoredPoses);
        playerPoses.Enqueue(playerLoc.position);
    }

    internal virtual void OnDisable()
    {
        transform.position = originalPos;
        body.rotation = originalRot;
        foundPlayer = false;
        lockedOnPlayer = false;
        canAttack = false;
        ResetAttack();
        fov = normalFOV;
        playerPoses.Clear();
        //Debug.Log("resetting");

    }

    internal virtual void OnValidate()
    {
        if (comfortableAttackDist < 0f)
        {
            Debug.LogWarning("Comfortable attack distance can't be smaller than 0!");
            comfortableAttackDist = 0f;
        }
        //if (comfortableAttackDist > minAttackDistance)
        //{
        //    Debug.LogWarning("Comfortable attack distance can't be greater than Min Attack Distance!");
        //    comfortableAttackDist = minAttackDistance;
        //}

        if (maxAttackDistance < minAttackDistance)
        {
            minAttackDistance = maxAttackDistance - 1;
        }
    }

    internal virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //cc = GetComponent<CircleCollider2D>();
        playerLoc = GameObject.Find("Player").transform;
        // set the ondied func
        if (TryGetComponent(out hp)) hp.OnHPZero = OnDied;
        originalPos = transform.position;
        originalRot = body.rotation;
    }

    internal virtual void Start()
    {
        NewDetectionRate();
        NewAttackDistance();
        moveSpeed = walkSpeed;
        rb.drag = deccelerationDrag;
        fov = normalFOV;
    }

    internal virtual void Update()
    {
        if (showDebugStuff) CalculateEnemyView();
        if (showForwards) Debug.DrawLine(transform.position,transform.position + body.up, Color.red, Time.deltaTime);

        HandleSight();


        if (chasePlayer)
        {
            // Player position recorder
            if ((timeSinceLastPos >= secondsBetweenPoses) || saveOneMorePos)
            {
                timeSinceLastPos = 0f;
                QueuePlayerPos();
            }

            // If we are chasing the player, look at the last known point
            if (!canSeeThroughWalls && !foundPlayer && !isAlerted && playerPoses.Count > 0f) LookAt(playerPoses.Last());
        }

        // base attack logic
        if (canAttack && attackReady)
        {
            attackReady = false;
            Attack();
        }
    }

    internal virtual void FixedUpdate()
    {
        // Standard chase player
        if (chasePlayer && lockedOnPlayer && PlayerDistance > attackDistance)
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
        // if player is getting too close
        else if (lockedOnPlayer && PlayerDistance < comfortableAttackDist)
        {
            moveSpeed = -retreatSpeed;
            bool isTooFast = Mathf.Abs(rb.velocity.magnitude) > Mathf.Abs(moveSpeed);
            if (!isTooFast) Move();
        }
        // if we are in the comfortable attack zone
        // or if we just havent discovered the player yet
        else
        {
            rb.drag = deccelerationDrag;

            if (lockedOnPlayer && !canAttack)
            {
                canAttack = true;
            }
        }


        // if we lost the player and is following their tacks
        if (chasePlayer && !foundPlayer && playerPoses.Count > 0f)
        {
            float distCovered = (Time.time - lerpStartTime) * runSpeed;
            transform.position = Vector3.Lerp(lerpStart, nextPos, distCovered / totalTravelDist);

            if (Vector3.Distance(transform.position, nextPos) < 0.05f)
            {
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
                else
                {
                    StartCoroutine(nameof(InspectSurroundings));
                }
            }
        }
    }

    internal virtual void HandleSight()
    {
        // Perform detection in intervals for performance
        if (frames >= detectionRate)
        {
            frames = 0;

            // if the player is too close and we havent see them yet - called once
            if (PlayerDistance <= instantDetectDist && !lockedOnPlayer)
            {
                lockedOnPlayer = true;
                rotationTime = 0f;
                //Debug.Log("too close");
            }

            // basic can see player check
            if (CanSeePlayer())
            {
                // if its the first time we see the player - called once
                if (!foundPlayer)
                {
                    // Immediately store the player's position
                    if (chasePlayer)
                    {
                        playerPoses.Clear();
                        QueuePlayerPos();
                    }
                    
                    foundPlayer = true;
                    rotationTime = 0f;
                    //fov = normalFOV;

                    Invoke(nameof(OnPlayerDiscovered), reactionTime);
                    //Debug.Log("found player");
                }
            }
            // if we can't see the player anymore - called once
            else if (foundPlayer)
            {
                // cancel any ongoing inspeciton
                if (isInspecting) StopAllCoroutines();

                // increase the detection rate
                if (chasePlayer) NewDetectionRate(1/searchDetectionRateMult);

                // for children to add their functionalities
                OnLostSightOfPlayer();

                //Debug.Log("lost player");
                // Change some detection related properties
                foundPlayer = false;
                if (!canSeeThroughWalls) lockedOnPlayer = false;
                rotationTime = 0f;
                moveSpeed = walkSpeed;
                if (chasePlayer) fov = searchFOV;
                
                if (playerPoses.Count > 0 && chasePlayer)
                {
                    // finding the quickest route
                    Queue<Vector3> tempQ = new();
                    Stack<Vector3> tempStk = new();
                    Vector3 shortcut = Vector3.zero;

                    // Scan our recorded player poses from first to last
                    foreach (Vector3 pos in playerPoses.Reverse())
                    {
                        bool clear = false;
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, (pos - transform.position).normalized, viewDistance, whatIsWall);
                        if (showDebugStuff) Debug.DrawLine(transform.position, pos, Color.blue, 2f);                                                                        // <--- debug.drawline
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
                        if (showDebugStuff) Debug.DrawLine(transform.position, shortcut, Color.yellow, 5f);                                                                     // <--- debug.drawline

                        int len = tempStk.Count;
                        for (int i = 0; i < len; i++)
                        {
                            tempQ.Enqueue(tempStk.Pop());
                        }
                        if (tempQ.Count > 0) playerPoses = tempQ;
                        //Debug.Log($"hold {tempStk.Count}");                                                                                                 // <- debug.logs
                        //Debug.Log($"temp {tempQ.Count}");
                        //Debug.Log($"playerposes {playerPoses.Count}");
                    }

                    saveOneMorePos = true;

                    nextPos = playerPoses.Peek();

                    // set the next position to go to
                    totalTravelDist = Vector3.Distance(transform.position, nextPos);
                    lerpStartTime = Time.time;
                    lerpStart = transform.position;
                    rotationTime = 0f;

                    // for debugging
                    if (showDebugStuff)
                    {
                        foreach (Vector3 item in playerPoses)
                        {
                            DrawDebugCross(item);
                        }
                    }
                }
            }
        }

        // if we are already looking at player - called every frame
        if (lockedOnPlayer)
        {
            //if (LookAt(playerLoc.position)) rotationTime = 0f;
            LookAt(playerLoc.position);

            timeSinceLastPos += Time.deltaTime;
        }
        frames++;
    }

    // lerped rotation for smoothness. Must be called from Update(). Returns true when completed
    internal virtual bool LookAt(Vector3 point) 
    {
        if (lockRotation) return true;

        rotationTime++;
        Vector3 thing = point - transform.position;
        Quaternion newQuat = Quaternion.Euler(0f, 0f, Vector3.SignedAngle(thing, Vector3.up, Vector3.back));
        body.rotation = Quaternion.Lerp(body.rotation, newQuat, rotationCurve.Evaluate(rotationTime * lookSpeed));
        return rotationTime * lookSpeed > 1f;
    }

    internal virtual IEnumerator InspectSurroundings()
    {
        isInspecting = true;

        // speed up detection rate even more
        NewDetectionRate(1 / (searchDetectionRateMult * 1.5f));

        //Debug.Log("Start inspect");
        Vector3 right = transform.position - transform.up;
        Vector3 left = transform.position + transform.up;
        Vector3 forward = transform.position + body.up;

        rotationTime = 0;
        while (!LookAt(right))
        {
            //Debug.Log("inspecting right");
            yield return null;
        }
        rotationTime = 0;
        while (!LookAt(left))
        {
            //Debug.Log("inspecting left");
            yield return null;
        }
        rotationTime = 0;
        while (!LookAt(forward))
        {
            //Debug.Log("inspecting forwards");
            yield return null;
        }

        // when finished chasing, reset detection
        NewDetectionRate();
        fov = normalFOV;
        isInspecting = false;
    }

    internal virtual void OnPlayerDiscovered()
    {
        if (!foundPlayer) return;

        // lock on to player
        lockedOnPlayer = true;
        moveSpeed = runSpeed;
        NewAttackDistance();
    }

    internal virtual void OnLostSightOfPlayer()
    {
        canAttack = false;
    }

    internal virtual void NewDetectionRate(float multiplier = 1f)
    {
        detectionRate = Mathf.RoundToInt(Random.Range(detectionRate + 10, detectionRate - 10) * multiplier);
    }

    internal virtual void NewAttackDistance()
    {
        attackDistance = Random.Range(minAttackDistance, maxAttackDistance);
    }

    internal virtual void CalculateEnemyView()
    {
        // "cone" angle debug lines
        float rads = Mathf.Deg2Rad * fov * 0.5f;
        float rads2 = Mathf.Deg2Rad * (360f - fov*0.5f);
        float x = body.up.x, y = body.up.y;

        sightMax = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));
        sightMin = new Vector2((Mathf.Cos(rads2) * x) - (Mathf.Sin(rads2) * y), (Mathf.Sin(rads2) * x) + (Mathf.Cos(rads2) * y));

        Debug.DrawLine(transform.position, transform.position + sightMax * viewDistance, Color.red, Time.deltaTime);

        Debug.DrawLine(transform.position, transform.position + body.up, Color.red, Time.deltaTime);
        
        Debug.DrawLine(transform.position, transform.position + sightMin * viewDistance, Color.red, Time.deltaTime);
    }

    public bool CanSeePlayer()
    {
        if (PlayerDistance <= viewDistance)
        {
            if (Vector3.Angle(body.up, playerLoc.position - transform.position) <= fov * 0.5f)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, (playerLoc.position - transform.position).normalized, viewDistance, whatBlocksSight);
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
        rb.drag = accelerationDrag;
        if (moveSpeed > 0) rb.AddForce(body.up * moveForce * rb.mass, ForceMode2D.Force);
        else rb.AddForce(body.up * -moveForce, ForceMode2D.Force);
    }

    internal virtual void DrawDebugCross(Vector3 pos, float duration = 1f, float segmentLength = 0.3f)
    {
        Debug.DrawLine(pos, pos + Vector3.up * segmentLength, Color.green, duration);
        Debug.DrawLine(pos, pos + Vector3.down * segmentLength, Color.green, duration);
        Debug.DrawLine(pos, pos + Vector3.left * segmentLength, Color.green, duration);
        Debug.DrawLine(pos, pos + Vector3.right * segmentLength, Color.green, duration);
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

    public void OnDied()
    {
        LevelManager.EnemyDied();
        gameObject.SetActive(false);
        //Debug.Log("doed");
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
}
