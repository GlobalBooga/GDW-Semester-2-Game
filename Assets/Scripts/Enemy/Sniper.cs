using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
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
    public Animator animator;
    public float animationLenght;
    bool isOn;


    const string LASER_SHOT_ANIM = "SniperLaserShot";
    const string EMPTY = "Empty";


    internal override void Awake()
    {
        base.Awake();
    }

    internal override void Start()
    {
        base.Start();
        lineRenderer.enabled = false;
    }

    internal override void Update()
    {
        base.Update();
        if (isOn) ShootLaser();
        //if (!isAttacking && isOn) TurnOff();
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
        StartCoroutine(nameof(Aim));
    }

    private IEnumerator Aim()
    {
        Debug.Log("aim");
        // aim delay
        yield return new WaitForSeconds(aimStartDelay);

        // turn on laser
        TurnOn();

        //keep aiming
        yield return new WaitForSeconds(aimTime);

        // warning flashes
        int flashes = 0;
        while (flashes < warningFlashes)
        {
            //on the last flash, freeze look
            if (flashes == warningFlashes - 1) 
            {
                lockRotation = true;
                rotationTime = 0;
            }

            if (isOn)
            {
                // turn off laser
                TurnOff();
                yield return new WaitForSeconds(laserFlashOffTime);
            }
            else
            {
                // turn on laser
                TurnOn();
                flashes++;
                yield return new WaitForSeconds(laserFlashOnTime);
            }
        }
        if (!isOn) TurnOn(); // turn on laser

        // shoot

        RaycastHit2D hit = Physics2D.Raycast(laserStart.position, body.right, 100f, whatTakesDamage);
        if (hit)
        {
            HPComponent hp;
            if (hit.transform.gameObject.TryGetComponent(out hp))
            {
                hp.Reduce(damage);
            }
        }

        if (animator) animator.Play(LASER_SHOT_ANIM);
        yield return new WaitForSeconds(animationLenght);

        TurnOff();

        lockRotation = false;
        yield return new WaitForSeconds(delayBetweenShots);
        if (animator) animator.Play(EMPTY);
        ResetAttack();
    }




    // call in update
    void ShootLaser()
    {
        RaycastHit2D hit = Physics2D.Raycast(laserStart.position, body.right, maxDist, whatBlocksSight);
        if (hit)
        {
            DrawLaser(laserStart.position - transform.position, hit.point - (Vector2)transform.position);
        }
        else
        {
            DrawLaser(laserStart.position - transform.position, laserStart.position - transform.position + (body.right * maxDist));
        }
    }

    void DrawLaser(Vector2 startPos, Vector2 endPos)
    {
        lineRenderer.SetPosition(0, startPos);
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

    internal override void OnLostSightOfPlayer()
    {
        base.OnLostSightOfPlayer();
        base.ResetAttack();
    }
}