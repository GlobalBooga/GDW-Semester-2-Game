using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "OnDetectedPlayer", story: "On detected player", category: "Action", id: "c3147c701b2aad419ba7ff9e455b33ce")]
public partial class OnDetectedPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<UnityEngine.AI.NavMeshAgent> Agent;
    [SerializeReference] public BlackboardVariable<EnemyScriptableObject> Eso;

    protected override Status OnStart()
    {
        Agent.Value.speed = Eso.Value.defaultRunSpeed;
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

