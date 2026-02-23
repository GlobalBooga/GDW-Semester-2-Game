using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "PlayerTooClose", story: "[Player] is too close | [Body], [Eso]", category: "Conditions", id: "d968ffb33291837cdcb372f0ceb933aa")]
public partial class PlayerTooCloseCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Transform> Player;
    [SerializeReference] public BlackboardVariable<GameObject> Body;
    [SerializeReference] public BlackboardVariable<EnemyScriptableObject> Eso;
    
    public override bool IsTrue()
    {
        return Vector2.Distance(Player.Value.position, Body.Value.transform.position) <= Eso.Value.instantDetectDist;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
