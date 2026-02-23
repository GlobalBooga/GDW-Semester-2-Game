using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CanSeePlayer", story: "[Agent] Sees [Player]", category: "Action/Conditional", id: "35f14e82354ae249516229937e87f7e3")]
public partial class CanSeePlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<Transform> Body;
    [SerializeReference] public BlackboardVariable<EnemyScriptableObject> Eso;

    private EnemyScriptableObject eso;
    private Transform playerLoc;
    private Transform transform;
    private Transform body;
    private float PlayerDistance;
    
    protected override Status OnStart()
    {
        eso = Eso.Value;
        playerLoc = Player.Value.transform;
        transform = Agent.Value.transform;
        body = Body.Value;
        
        PlayerDistance = Vector3.Distance(transform.position, playerLoc.position);
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (PlayerDistance <= eso.viewDistance)
        {
            // is in fov?
            if (Vector3.Angle(body.up, playerLoc.position - transform.position) <= eso.normalFOV * 0.5f)
            {
                // is view blocked?
                RaycastHit2D hit = Physics2D.Raycast(transform.position, (playerLoc.position - transform.position).normalized, eso.viewDistance, eso.whatBlocksSight);
                if (hit)
                {
                    if (hit.transform.gameObject.name == "Player")
                    {
                        return Status.Success;
                    }
                }
            }
        }
        return Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

