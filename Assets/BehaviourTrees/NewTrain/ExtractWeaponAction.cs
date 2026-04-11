using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ExtractWeapon", story: "Extract [weapons]", category: "Action", id: "2d68e150ff30c17f397162c2575da6f4")]
public partial class ExtractWeaponAction : Action
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> Weapons;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

