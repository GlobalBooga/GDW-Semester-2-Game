using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Enemy
{

    public Transform laserStart;
    public LineRenderer lineRenderer;
    public Animator animator;

    private bool isLaserOn;
    private const string LASER_SHOT_ANIM = "SniperLaserShot";
    private const string EMPTY = "Empty";
    private SniperScriptableObject sso;


    internal override void Awake()
    {
        base.Awake();
        sso = (SniperScriptableObject)eso;
    }

    internal override void Start()
    {
        base.Start();
        lineRenderer.enabled = false;
    }

    internal override void Update()
    {
        base.Update();
        if (isLaserOn) ShootLaser();
        //if (!isAttacking && isOn) TurnOff();
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
        //Debug.Log("aim");
        // aim delay
        yield return new WaitForSeconds(sso.aimStartDelay);

        // turn on laser
        TurnOn();

        //keep aiming
        yield return new WaitForSeconds(sso.aimTime);

        // warning flashes
        int flashes = 0;
        while (flashes < sso.warningFlashes)
        {
            //on the last flash, freeze look
            if (flashes == sso.warningFlashes - 1) 
            {
                lockRotation = true;
                rotationTime = 0;
            }

            if (isLaserOn)
            {
                // turn off laser
                TurnOff();
                yield return new WaitForSeconds(sso.laserFlashOffTime);
            }
            else
            {
                // turn on laser
                TurnOn();
                flashes++;
                yield return new WaitForSeconds(sso.laserFlashOnTime);
            }
        }
        if (!isLaserOn) TurnOn(); // turn on laser


        // shoot

        RaycastHit2D hit = Physics2D.Raycast(laserStart.position, body.up, 100f, sso.whatTakesDamage);
        if (hit)
        {
            StaticHelpers.ApplyDamage(hit.transform.gameObject, sso.damage);
        }

        // alert everyone
        LevelManager.AlertAllEnemiesInCurrentScene(transform.position);

        // Play animation
        if (animator) animator.Play(LASER_SHOT_ANIM);
        yield return new WaitForSeconds(sso.animationLenght);

        TurnOff();

        lockRotation = false;
        yield return new WaitForSeconds(sso.delayBetweenShots);
        if (animator) animator.Play(EMPTY);
        ResetAttack();
    }




    // call in update
    void ShootLaser()
    {
        RaycastHit2D hit = Physics2D.Raycast(laserStart.position, body.up, sso.maxDist, sso.whatBlocksSight);
        if (hit)
        {
            DrawLaser(laserStart.position - transform.position, hit.point - (Vector2)transform.position);
        }
        else
        {
            DrawLaser(laserStart.position - transform.position, laserStart.position - transform.position + (body.up * sso.maxDist));
        }
    }

    void DrawLaser(Vector2 startPos, Vector2 endPos)
    {
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    public void TurnOn()
    {
        if (!isLaserOn)
        {
            isLaserOn = true;
            lineRenderer.enabled = true;
        }
    }
    
    public void TurnOff()
    {
        if (isLaserOn)
        {
            isLaserOn = false;
            lineRenderer.enabled = false;
        }
    }

    internal override void OnLostSightOfPlayer()
    {
        base.OnLostSightOfPlayer();
        if (isAttacking) StopCoroutine(nameof(Aim));
        ResetAttack();
    }

    internal override void ResetAttack()
    {
        base.ResetAttack();
        if (isLaserOn) TurnOff();
    }
}