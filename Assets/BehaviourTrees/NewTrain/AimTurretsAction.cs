using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AimTurrets", story: "Aim at [target]", category: "Action", id: "c987f8ecc1803546c59a11207e8670db")]
public partial class AimTurretsAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> target;

    [SerializeReference] public BlackboardVariable<TrainBossScriptableObject> Params;
    [SerializeReference] public BlackboardVariable<List<GameObject>> turrets;
    
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        foreach (var item in turrets.Value)
        {
            if (item.activeSelf)
            {
                Vector2 t = target.Value.position - item.transform.position;
                float angle = Vector3.SignedAngle(t, -item.transform.right, Vector3.back);

                item.transform.rotation = Quaternion.Euler(0, 0, 
                    Mathf.Clamp(item.transform.rotation.eulerAngles.z + 
                                (angle * Time.deltaTime * Params.Value.rotationSpeed), 90-Params.Value.maxAngle, 90+Params.Value.maxAngle));
            }
        }
        
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

