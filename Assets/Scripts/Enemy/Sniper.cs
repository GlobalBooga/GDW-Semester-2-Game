using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Sniper : Enemy
{
    [Header("Sniper"), Space(5f)]
    public float damage;
    public float delayBetweenShots = 1f;
    public float aimStartDelay = 0.5f;
    public float aimTime = 3f;
    public int warningFlashes = 3;
    public float laserFlashOnTime = 0.25f;
    public float laserFlashOffTime = 0.25f;

    [Space(10)]
    
    [Header("Laser"), Space(5f)]
    public float maxDist = 100f;
    public Transform laserStart;
    public LineRenderer lineRenderer;
    bool isOn;


    internal override void Awake()
    {
        base.Awake();
    }

    internal override void Start()
    {
        base.Start();
        //lineRenderer.enabled = false;
    }

    internal override void Update()
    {
        base.Update();
        if (isOn) ;
        ShootLaser();
    }

    internal override void OnValidate()
    {
        base.OnValidate();
    }

    internal override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    internal override void Attack()
    {
        base.Attack();
        //StartCoroutine(nameof(Aim));
    }

    private IEnumerator Aim()
    {
        while (CanSeePlayer())
        {
            // aim delay
            Debug.Log("aim delay");
            yield return new WaitForSeconds(aimStartDelay);

            // turn on laser
            Debug.Log("turn on laser");
            TurnOn();

            // slow down rotation time
            lookSpeed = 0.001f;

            //keep aiming
            yield return new WaitForSeconds(aimTime);

            // warning flashes
            bool laseron = true;
            int flashes = 0;
            while (flashes++ < warningFlashes)
            {
                if (laseron)
                {
                    Debug.Log("flash");
                    // turn off laser
                    TurnOff();
                    yield return new WaitForSeconds(laserFlashOffTime);
                }
                else
                {
                    // turn on laser
                    TurnOn();
                    yield return new WaitForSeconds(laserFlashOnTime);
                }
            }
            if (!laseron) TurnOn(); // turn on laser

            // shoot

            Debug.Log("shoot");
            RaycastHit2D hit = Physics2D.Raycast(laserStart.position, body.right, 100f, whatTakesDamage);
            if (hit)
            {
                HPComponent hp;
                if (hit.transform.gameObject.TryGetComponent(out hp))
                {
                    hp.Reduce(damage);
                }
            }

            //Debug.Log("shoot");
            TurnOff();
            // reset look speed
            lookSpeed = 0.01f;
            yield return new WaitForSeconds(delayBetweenShots);
        }
    }




    // call in update
    void ShootLaser()
    {
        RaycastHit2D hit = Physics2D.Raycast(laserStart.position, body.right, maxDist);
        if (hit)
        {
            DrawLaser(laserStart.position, hit.point);
        }
        else
        {
            DrawLaser(laserStart.position, laserStart.position + (body.right * maxDist));
        }
    }

    void DrawLaser(Vector2 startPos, Vector2 endPos)
    {
        //lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    public void TurnOn()
    {
        if (!isOn)
        {
            isOn = true;
            lineRenderer.enabled = true;
        }
    }
    
    public void TurnOff()
    {
        if (isOn)
        {
            isOn = false;
            lineRenderer.enabled = false;
        }
    }
}