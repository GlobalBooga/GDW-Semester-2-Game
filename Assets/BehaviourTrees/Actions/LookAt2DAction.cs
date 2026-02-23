using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "LookAt2D", story: "[Self] looks at [target]", category: "Action", id: "c61582f908403148863ad1477259b36d")]
public partial class LookAt2DAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Self;
    [SerializeReference] public BlackboardVariable<Transform> Target;

    [SerializeReference] public BlackboardVariable<bool> Continuous = new BlackboardVariable<bool>(false);
    
    protected override Status OnStart()
    {
        if (Self.Value == null || Target.Value == null)
        {
            LogFailure($"Missing Transform or Target.");
            return Status.Failure;
        }

        ProcessLookAt();
        return Continuous.Value ? Status.Running : Status.Success;
    }

    protected override Status OnUpdate()
    {
        if (Continuous.Value)
        {
            ProcessLookAt();
            return Status.Running;
        }
        return Status.Success;
    }

    void ProcessLookAt()
    {
        Vector3 targetPosition = Target.Value.position;
        
        Quaternion newQuat = Quaternion.Euler(0f, 0f, Vector3.SignedAngle(targetPosition - Self.Value.position, Vector3.up, Vector3.back));
        Self.Value.rotation = Quaternion.Lerp(Self.Value.rotation, newQuat, 0.1f);
        
        //Self.Value.rotation = Quaternion.LookRotation(Self.Value.forward, Self.Value.up);
    }
}

