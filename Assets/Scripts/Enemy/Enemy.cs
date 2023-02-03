using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Detection"), Space(5f)]
    public float reactionTime = 1f;
    public float viewDistance = 10f;
    public float fov = 60f;
    private bool foundPlayer;
    private bool lockedOnPlayer;
    private float lookSpeed = 0.01f;
    public AnimationCurve curve;

    private Queue<Vector3> playerPoses = new();
    private const int maxStoredPoses = 4;
    private const float secondsBetweenPoses = 0.5f;
    private float timeSinceLastPos = 0;

    private float totalTravelDist;
    private float lerpStartTime;
    private Vector3 lerpStart;
    private Vector3 nextPos;
    private float minDistanceBetweenPoses = 1f;
    private bool saveOneMorePos;

    [Space(10f)]

    [Header("Movement"), Space(5f)]
    public float walkSpeed = 3f;
    public float runSpeed = 8f;
    private float moveSpeed;
    public float moveForce = 15f;
    public float accelerationDrag = 1f;
    public float deccelerationDrag = 5f;
    private bool isOnEdge;
    [Space(10f)]

    [Header("Aggressive Behaviour"), Space(5f)]
    public float maxAttackDistance = 10f;
    public float minAttackDistance = 4f;
    private float attackDistance;

    [Space(10f)]

    [Header("Masks"), Space(5f)]
    public LayerMask whatIsPlayer;
    public LayerMask whatIsWall;
    public LayerMask whatBlocksSight;
    [Space(10f)]

    // OBJECTS
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private CircleCollider2D cc;
    private Transform playerLoc;

    private int frames;
    private int detectionRate = 50; // the average
    Vector3 sightMax;
    Vector3 sightMin;
    private float rotationTime;

    Vector3 PlayerDirection => (playerLoc.position - transform.position);
    private float PlayerDistance => Vector3.Distance(transform.position, playerLoc.position);

    
    private void QueuePlayerPos()
    {
        // If the player is too close from the last logged position
        if (playerPoses.Count > 0)
        {
            if (Vector3.Distance(playerLoc.position, playerPoses.Last()) < minDistanceBetweenPoses) return;
        }

        saveOneMorePos = false;

        if (playerPoses.Count > maxStoredPoses) playerPoses.Dequeue();
        DrawDebugCross(playerLoc.position, secondsBetweenPoses * maxStoredPoses);
        playerPoses.Enqueue(playerLoc.position);
    }



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();
        playerLoc = GameObject.Find("Player").transform;

    }

    private void Start()
    {
        NewDetectionRate();
        NewAttackDistance();
        moveSpeed = walkSpeed;
        rb.drag = deccelerationDrag;
    }

    void Update()
    {
        CalculateEnemyView();
        HandleSight();

        // Player position recorder
        if ((timeSinceLastPos >= secondsBetweenPoses) || saveOneMorePos)
        {
            timeSinceLastPos = 0f;
            QueuePlayerPos();
        }

        // If we are chasing the player, look at the next point
        if (!foundPlayer && playerPoses.Count > 0f) LookAt(nextPos);
    }

    private void FixedUpdate()
    {
        // Standard chase player
        if (lockedOnPlayer && PlayerDistance > attackDistance)
        {
            bool isTooFast = Mathf.Abs(rb.velocity.magnitude) > moveSpeed;
            if (!isTooFast) Move();
        }
        else
        {
            rb.drag = deccelerationDrag;
        }



        // if we lost the player
        if (!foundPlayer && playerPoses.Count > 0f)
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

    private void HandleSight()
    {
        // Perform detection in intervals for performance
        if (frames >= detectionRate)
        {
            frames = 0;
            // basic can see player check
            if (CanSeePlayer())
            {
                // if its the first time we see the player - called once
                if (!foundPlayer)
                {
                    // Immediately store the player's position
                    QueuePlayerPos();
                    
                    foundPlayer = true;
                    rotationTime = 0f;
                    Invoke(nameof(OnPlayerDiscovered), reactionTime);
                    //Debug.Log("found player");
                }
            }
            // if we can't see the player anymore - called once
            else if (foundPlayer)
            {
                // cancel any ongoing inspeciton
                StopAllCoroutines();

                // double the detection rate
                NewDetectionRate(0.5f);

                //Debug.Log("lost player");
                foundPlayer = false;
                lockedOnPlayer = false;
                rotationTime = 0f;
                moveSpeed = walkSpeed;

                
                if (playerPoses.Count > 0)
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
                        Debug.DrawLine(transform.position, pos, Color.blue, 2f);                                                                        // <--- debug.drawline
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
                        Debug.DrawLine(transform.position, shortcut, Color.yellow, 5f);                                                                     // <--- debug.drawline

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

                    foreach (Vector3 item in playerPoses)
                    {
                        DrawDebugCross(item);
                    }
                }
            }
        }

        // if we are already looking at player - called every frame
        if (lockedOnPlayer)
        {
            LookAt(playerLoc.position);

            timeSinceLastPos += Time.deltaTime;
        }
        frames++;
    }


    // lerped rotation for smoothness. Must be called from Update(). Returns true when completed
    private bool LookAt(Vector3 point) 
    {
        rotationTime++;
        Vector3 thing = point - transform.position;
        Quaternion newQuat = Quaternion.Euler(0f, 0f, Vector3.SignedAngle(thing, Vector3.right, Vector3.back));
        transform.rotation = Quaternion.Lerp(transform.rotation, newQuat, curve.Evaluate(rotationTime * lookSpeed));
        return rotationTime * lookSpeed > 1f;
    }


    private IEnumerator InspectSurroundings()
    {
        // speed up detection even more for this
        NewDetectionRate(0.25f);

        //Debug.Log("Start inspect");
        Vector3 right = transform.position - transform.up;
        Vector3 left = transform.position + transform.up;
        Vector3 forward = transform.position + transform.right;

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
    }


    private void OnPlayerDiscovered()
    {
        if (!foundPlayer) return;

        // lock on to player
        lockedOnPlayer = true;
        moveSpeed = runSpeed;
        NewAttackDistance();
    }

    private void NewDetectionRate(float multiplier = 1f)
    {
        detectionRate = Mathf.RoundToInt(Random.Range(detectionRate + 10, detectionRate - 10) * multiplier);
    }

    private void NewAttackDistance()
    {
        attackDistance = Random.Range(minAttackDistance, maxAttackDistance);
    }

    private void CalculateEnemyView()
    {
        // "cone" angle debug lines
        float rads = Mathf.Deg2Rad * fov * 0.5f;
        float rads2 = Mathf.Deg2Rad * (360f - fov*0.5f);
        float x = transform.right.x, y = transform.right.y;

        sightMax = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));
        sightMin = new Vector2((Mathf.Cos(rads2) * x) - (Mathf.Sin(rads2) * y), (Mathf.Sin(rads2) * x) + (Mathf.Cos(rads2) * y));

        Debug.DrawLine(transform.position, transform.position + sightMax * viewDistance, Color.red, Time.deltaTime);

        Debug.DrawLine(transform.position, transform.position + transform.right, Color.red, Time.deltaTime);
        
        Debug.DrawLine(transform.position, transform.position + sightMin * viewDistance, Color.red, Time.deltaTime);
    }

    private bool CanSeePlayer()
    {
        if (PlayerDistance <= viewDistance)
        {
            if (Vector3.Angle(transform.right, playerLoc.position - transform.position) <= fov * 0.5f)
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

    private void Move()
    {
        rb.drag = accelerationDrag;
        rb.AddForce(transform.right * moveForce, ForceMode2D.Force);
    }

    private void DrawDebugCross(Vector3 pos, float duration = 1f, float segmentLength = 0.3f)
    {
        Debug.DrawLine(pos, pos + Vector3.up * segmentLength, Color.green, duration);
        Debug.DrawLine(pos, pos + Vector3.down * segmentLength, Color.green, duration);
        Debug.DrawLine(pos, pos + Vector3.left * segmentLength, Color.green, duration);
        Debug.DrawLine(pos, pos + Vector3.right * segmentLength, Color.green, duration);
    }
}
