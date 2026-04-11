using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.Serialization;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "LaserAttack", story: "Laser", category: "Action", id: "12138c0397d420b2c71a011b3a5434c7")]
public partial class LaserAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<AndarozScriptableObject> Params;
    [SerializeReference] public BlackboardVariable<Rigidbody2D> rb;
    [SerializeReference] public BlackboardVariable<HPComponent> hp;
    [SerializeReference] public BlackboardVariable<Animator> torsoAnimator;
    [SerializeReference] public BlackboardVariable<Laser> rightLaser;
    [SerializeReference] public BlackboardVariable<Laser> leftLaser;
    [SerializeReference] public BlackboardVariable<Transform> rightLaserStart;
    [SerializeReference] public BlackboardVariable<Transform> leftLaserStart;

    private float duration;
    private float startDelay;
    private bool isFirstTime = true;
    private bool laserPrepped;
    
    protected override Status OnStart()
    {
        laserPrepped = false;
        
        // stop
        rb.Value.linearVelocity = Vector2.zero;

        hp.Value.isInvincible = true;

        // animation - arms out
        if (isFirstTime) torsoAnimator.Value.Play(Andaroz.LASER_ATTACK);
        else torsoAnimator.Value.Play(Andaroz.LASER_ATTACK_LONG);

        startDelay = Params.Value.delayParticlesToLaser;

        duration = isFirstTime ? Params.Value.laser_duration_firstTime : Params.Value.laser_duration;
        
        return Status.Running;
    }
    
    protected override Status OnUpdate()
    {
        if ((startDelay -= Time.deltaTime) > 0) return Status.Running;
        if ((duration -= Time.deltaTime) < 0) return Status.Success;
        
        if (!laserPrepped)
        {
            laserPrepped = true;
            rightLaser.Value.TurnOn();    
            leftLaser.Value.TurnOn();

            float intensity = isFirstTime
                ? Params.Value.laser_cameraShakeIntensity_firstTime
                : Params.Value.laser_cameraShakeIntensity;
            float time = isFirstTime
                ? Params.Value.laser_cameraShakeTime_firstTime
                : Params.Value.laser_cameraShakeTime;
            
            CameraShake.instance.ShakeCamera(intensity, time);
        }

        ShootLaser();

        return Status.Running;
    }

    protected override void OnEnd()
    {
        rightLaser.Value.TurnOff();
        leftLaser.Value.TurnOff();
        
        hp.Value.isInvincible = false;
        isFirstTime = false;
    }
    
    // call every frame
    void ShootLaser()
    {
        RaycastHit2D hitR = Physics2D.Raycast(rightLaserStart.Value.position, -torsoAnimator.Value.transform.up, 100f, Params.Value.whatBlocksSight);
        RaycastHit2D hitL = Physics2D.Raycast(leftLaserStart.Value.position, torsoAnimator.Value.transform.up, 100f, Params.Value.whatBlocksSight);
        if (hitR)
        {
            if (hitR.collider.gameObject.layer == StaticHelpers.PlayerLayer) StaticHelpers.ApplyDamage(hitR.collider.gameObject, Params.Value.laser_damage);
            else if (Params.Value.laser_pillarDamageMultiplier > 0) StaticHelpers.ApplyDamage(hitR.transform.gameObject, Params.Value.laser_damage * Params.Value.laser_pillarDamageMultiplier);
            rightLaser.Value.DrawLaser(rightLaserStart.Value.position, hitR.point);
        }
        if (hitL)
        {
            if (hitL.collider.gameObject.layer == StaticHelpers.PlayerLayer) StaticHelpers.ApplyDamage(hitL.collider.gameObject, Params.Value.laser_damage);
            else if (Params.Value.laser_pillarDamageMultiplier > 0) StaticHelpers.ApplyDamage(hitL.transform.gameObject, Params.Value.laser_damage * Params.Value.laser_pillarDamageMultiplier);
            leftLaser.Value.DrawLaser(leftLaserStart.Value.position, hitL.point);
        }
    }
}

