using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Recenter", story: "Recenter", category: "Action", id: "e7e57f745b6886826fe7866452f6c01e")]
public partial class RecenterAction : Action
{
    [SerializeReference] public BlackboardVariable<TrainBossScriptableObject> Params;
    [SerializeReference] public BlackboardVariable<Rigidbody2D> rb;
    [SerializeReference] public BlackboardVariable<bool> shouldRecenter;
    
    
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (shouldRecenter.Value)
        {
            // return to center
            /*float currentX = CenterofFireZone();

            if (Mathf.Abs(currentX - ogX) > 1f)
            {
                if (!resettingpos)
                {
                    resettingpos = true;
                    rb.Value.constraints = RigidbodyConstraints2D.FreezePositionY;
                    rb.Value.freezeRotation = true;
                    posreset = false;
                }


                if (currentX - ogX > 0)
                {
                    rb.Value.AddForce(Vector2.left * Params.Value.moveSpeed * rb.Value.mass, ForceMode2D.Force);
                }
                else
                {
                    rb.Value.AddForce(Vector2.right * Params.Value.moveSpeed * rb.Value.mass, ForceMode2D.Force);
                }
            }
            else if (!posreset)
            {
                posreset = true;
                rb.Value.constraints = RigidbodyConstraints2D.FreezeAll;
            }*/
        }
        
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

