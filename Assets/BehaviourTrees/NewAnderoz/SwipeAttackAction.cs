using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SwipeAttack", story: "Swipe", category: "Action", id: "9e1e308b04a5f7c6eaffb6cced974400")]
public partial class SwipeAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Body;
    [SerializeReference] public BlackboardVariable<AndarozScriptableObject> Params;
    [SerializeReference] public BlackboardVariable<Animator> TorsoAnimator; 
    [SerializeReference] public BlackboardVariable<bool> LockRotation; 
    private float applyDamageCountdown;
    
    protected override Status OnStart()
    {
        LockRotation.Value = true;
        applyDamageCountdown = Params.Value.swipe_applyDmgDelay;
        TorsoAnimator.Value.Play(Andaroz.SWIPE_ATTACK);
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if ((applyDamageCountdown -= Time.deltaTime) > 0) return Status.Running;

        // if were still within range of swipe
        Collider2D[] cols = Physics2D.OverlapCircleAll(Body.Value.position, Params.Value.swipe_maxAttackDistance, Params.Value.whatTakesDamage);
        if (cols.Length > 0)
        {
            foreach (var item in cols)
            {
                if (Vector3.Angle(Body.Value.up, item.transform.position - Body.Value.position) < Params.Value.swipe_angle / 2f)
                {
                    if (item.gameObject.layer == StaticHelpers.PlayerLayer)
                    {
                        StaticHelpers.ApplyDamage(item.gameObject, Params.Value.swipe_damage);
                    }
                    else if (Params.Value.swipe_pillarDamageMultiplier > 0)
                    {
                        StaticHelpers.ApplyDamage(item.gameObject, Params.Value.swipe_damage * Params.Value.swipe_pillarDamageMultiplier);
                    }
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

