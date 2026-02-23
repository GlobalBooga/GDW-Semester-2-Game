using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Can See Player", story: "[Player] is visible | [Body], [Eso]", category: "Conditions", id: "a9262357a7362bbab1ab3265b92376df")]
public partial class CanSeePlayerCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<GameObject> Body;
    [SerializeReference] public BlackboardVariable<EnemyScriptableObject> Eso;

    private Transform playerLoc;
    private Transform transform;
    private float PlayerDistance;

    
    public override bool IsTrue()
    {
        if (!(PlayerDistance <= Eso.Value.viewDistance)) return false;
        
        // is in fov?
        if (!(Vector3.Angle(transform.up, playerLoc.position - transform.position) <= Eso.Value.normalFOV * 0.5f)) return false;
        
        // is view blocked?
        var hit = Physics2D.Raycast(transform.position, (playerLoc.position - transform.position).normalized, Eso.Value.viewDistance, Eso.Value.whatBlocksSight);
        
        if (!hit) return false;
        
        return hit.transform.gameObject.name == "Player";
    }

    public override void OnStart()
    {
        transform = Body.Value.transform;
        playerLoc = Player.Value.transform;
        PlayerDistance = Vector3.Distance(transform.position, playerLoc.position);
    }

    public override void OnEnd()
    {
    }
}
