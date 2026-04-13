using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ShowHPBar", story: "Show health bar", category: "Action", id: "e0d7ae7c32d69d6f72da53d541ab0bbd")]
public partial class ShowHpBarAction : Action
{

    protected override Status OnStart()
    {
        LevelManager.instance.startBossBattle = true;
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

