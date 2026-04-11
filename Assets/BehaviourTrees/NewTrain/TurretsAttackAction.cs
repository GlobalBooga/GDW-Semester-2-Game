using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TurretsAttack", story: "Shoot [X] times", category: "Action", id: "64218f48d0251f633b68e81e467e105c")]
public partial class TurretsAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<int> X;

    [SerializeReference] public BlackboardVariable<TrainBossScriptableObject> Params;
    public BlackboardVariable<List<GameObject>> turrets;
    
    private float shootStartDelay;
    private float shootDelay;
    private int shots;
    
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if ((shootStartDelay -= Time.deltaTime) > 0 || (shootDelay -= Time.deltaTime) > 0) return Status.Running;

        if (shots++ > X.Value) return Status.Success;
        
        for (int i = 0; i < Params.Value.shots_Turrets; i++)
        {
            foreach (var item in turrets.Value)
            {
                item.GetComponent<TrainTurret>().Fire(Params.Value.bullet, Params.Value.turrets_damage, Params.Value.bulletSpread, 
                    Params.Value.bulletSpeed, Params.Value.turrets_coverDamageMultiplier);
            }

        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

