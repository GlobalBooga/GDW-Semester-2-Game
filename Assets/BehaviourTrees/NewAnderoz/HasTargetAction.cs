using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.Serialization;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HasTarget", story: "Has Target", category: "Action/Conditional", id: "65372c2d66d83af0b5e61e5218bc6e5c")]
public partial class HasTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> target;
    
    protected override Status OnStart()
    {
        return target.Value ? Status.Success : Status.Failure;
    }
}

