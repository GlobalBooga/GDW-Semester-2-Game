using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Idle", story: "Idle", category: "Action", id: "0778a4a445b91ef913cb2517747835d7")]
public partial class IdleAction : Action
{
    [SerializeReference] public BlackboardVariable<Animator> TorsoAnimator; 

    protected override Status OnStart()
    {
        TorsoAnimator.Value.Play(Andaroz.IDLE);
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

