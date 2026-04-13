using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HealthCheck", story: "Is [max]% <= HP <=[min]%", category: "Action/Conditional", id: "e6156f35e228b8d654c6ffd7c092f269")]
public partial class HealthCheckAction : Action
{
    [SerializeReference] public BlackboardVariable<HPComponent> HpComponent;
    [SerializeReference] public BlackboardVariable<float> max;
    [SerializeReference] public BlackboardVariable<float> min;
    protected override Status OnStart()
    {
        float hp = HpComponent.Value.GetHealth() / HpComponent.Value.maxHealth * 100.0f;
        return (max.Value >= hp && hp >= min.Value) ? Status.Success : Status.Failure;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

