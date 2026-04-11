using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SlamAttack", story: "Slam", category: "Action", id: "43d8c14b5123d6d51ec77e508ae63cd5")]
public partial class SlamAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<AndarozScriptableObject> Params;
    [SerializeReference] public BlackboardVariable<Transform> SlamPoint;
    [SerializeReference] public BlackboardVariable<Animator> TorsoAnimator; 
    [SerializeReference] public BlackboardVariable<bool> LockRotation; 
    private float applyDamageCountdown;
    
    protected override Status OnStart()
    {
        applyDamageCountdown = Params.Value.slam_applyDmgDelay;
        LockRotation.Value = true;
        TorsoAnimator.Value.Play(Andaroz.SLAM_ATTACK);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if ((applyDamageCountdown -= Time.deltaTime) > 0) return Status.Running;
        
        GameObject.Instantiate(Params.Value.slam_Particles, SlamPoint.Value);

        CameraShake.instance.ShakeCamera(Params.Value.slam_cameraShakeIntensity, Params.Value.slam_cameraShakeTime);
        
        // Apply damage
        Collider2D[] cols = Physics2D.OverlapCircleAll(SlamPoint.Value.position, 5, Params.Value.whatTakesDamage);
        if (cols.Length > 0) 
        {
            foreach (var item in cols)
            {
                if (item.gameObject.layer == StaticHelpers.SpecialBreakableObjectLayer)
                {
                    if (Params.Value.slam_pillarDamageMultiplier > 0)
                    {
                        StaticHelpers.ApplyDamage(item.gameObject, Params.Value.slam_damage * Params.Value.slam_pillarDamageMultiplier);
                    }
                }
                else
                {
                    StaticHelpers.ApplyDamage(item.gameObject, Params.Value.slam_damage);
                }
            }
        }

        return Status.Success;
    }

    protected override void OnEnd()
    {
        LockRotation.Value = false;
    }
}

